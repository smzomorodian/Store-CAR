using Application.Command.User.Command;
using Infrustructure.Repository;
using Infrustructure.Repository.IRepository;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Command.User.CommandHandler
{
    public class ChangePasswordcommandHandler<T> : IRequestHandler<ChangePasswordcommand, string> where T : class
    {
        private readonly IRepository<T> _genericRepository;
        private readonly IUserInfoRepository<T> _userInfoRepository;

        // اصلاح نام سازنده
        public ChangePasswordcommandHandler(IRepository<T> genericRepository, IUserInfoRepository<T> userInfoRepository)
        {
            _genericRepository = genericRepository;
            _userInfoRepository = userInfoRepository;
        }

        public async Task<string> Handle(ChangePasswordcommand request, CancellationToken cancellationToken)
        {
            // بررسی خالی بودن مقادیر ورودی
            if (request == null || string.IsNullOrEmpty(request.NationalCode) ||
                string.IsNullOrEmpty(request.Otp) || string.IsNullOrEmpty(request.NewPassword))
            {
                return "کدملی، کد موقت و رمز جدید نمی‌توانند خالی باشند";
            }

            // یافتن کاربر از طریق کد ملی
            var user = await _userInfoRepository.getnationalcode(request.NationalCode);
            if (user == null)
            {
                return "کاربری با این کدملی پیدا نشد";
            }

            // مقایسه OTP
            if (user.otp != request.Otp)
            {
                return "کد موقت اشتباه است";
            }

            // بررسی انقضای OTP
            if (!user.OtpExpiry.HasValue || user.OtpExpiry < DateTime.Now)
            {
                return "کد موقت منقضی شده است";
            }

            // تغییر رمز عبور
            user.password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.Otp = null;
            user.OtpExpiry = null;

            try
            {
                // ذخیره تغییرات
                await _genericRepository.SavechangeAsync();
                return "رمز عبور با موفقیت تغییر کرد";
            }
            catch (Exception ex)
            {
                // اصلاح برگشت پیام خطا
                return $"خطا در تغییر رمز عبور: {ex.Message}";
            }
        }
    }
}
