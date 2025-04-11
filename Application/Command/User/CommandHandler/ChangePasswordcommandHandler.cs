using Application.Command.User.Command;
using Infrustructure.Repository.IRepository;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Usermodel = Domain.Model.User;

namespace Application.Command.User.CommandHandler
{
    public class ChangePasswordcommandHandler<T> : IRequestHandler<ChangePasswordcommand<T>, string> where T : Usermodel, new()
    {
        private readonly IRepository<T> _genericRepository;
        private readonly IUserInfoRepository<T> _userInfoRepository;

        public ChangePasswordcommandHandler(IRepository<T> genericRepository, IUserInfoRepository<T> userInfoRepository)
        {
            _genericRepository = genericRepository;
            _userInfoRepository = userInfoRepository;
        }

        public async Task<string> Handle(ChangePasswordcommand<T> request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrEmpty(request.NationalCode) ||
                string.IsNullOrEmpty(request.Otp) || string.IsNullOrEmpty(request.NewPassword))
            {
                return "کدملی، کد موقت و رمز جدید نمی‌توانند خالی باشند";
            }

            var user = await _userInfoRepository.getnationalcode(request.NationalCode);
            if (user == null)
                return "کاربری با این کدملی پیدا نشد";

            if (user.Otp != request.Otp)
                return "کد موقت اشتباه است";

            if (!user.OtpExpiry.HasValue || user.OtpExpiry < DateTime.Now)
                return "کد موقت منقضی شده است";

            user.password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.Otp = null;
            user.OtpExpiry = null;

            try
            {
                await _genericRepository.SavechangeAsync();
                return "رمز عبور با موفقیت تغییر کرد";
            }
            catch (Exception ex)
            {
                return $"خطا در تغییر رمز عبور: {ex.Message}";
            }
        }
    }
}
