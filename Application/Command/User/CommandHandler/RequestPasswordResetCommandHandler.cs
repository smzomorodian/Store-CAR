using Application.Command.User.Command;
using Infrustructure.Repository;
using Infrustructure.Repository.IRepository;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Usermodel = Domain.Model.UserModel.User;

namespace Application.Command.User.CommandHandler
{
    public class RequestPasswordResetCommandHandler<T> : IRequestHandler<RequestPasswordResetCommand<T>, string> where T : Usermodel
    {
        private readonly IRepository<T> _genericRepository;
        private readonly IUserInfoRepository<T> _userInfoRepository;
        

        public RequestPasswordResetCommandHandler(IRepository<T> genericRepository, IUserInfoRepository<T> userInfoRepository)
        {
            _genericRepository = genericRepository;
            _userInfoRepository = userInfoRepository;
        }

        public async Task<string> Handle(RequestPasswordResetCommand<T> request, CancellationToken cancellationToken)
        {
            // Check if NationalCode is empty
            if (string.IsNullOrEmpty(request.nationalCode))
            {
                return "کد ملی نمی‌تواند خالی باشد";
            }

            // Fetch the user using the national code
            var user = await _userInfoRepository.getnationalcode(request.nationalCode);
            if (user == null)
            {
                return "کاربری با این کد ملی پیدا نشد";
            }

            // Generate OTP
            var random = new Random();
            string otp = random.Next(100000, 999999).ToString();
            user.Otp = otp;
            user.OtpExpiry = DateTime.Now.AddMinutes(5);

            try
            {
                // Save changes to the repository
                await _genericRepository.SavechangeAsync();

                // Send OTP (you should implement this method to send the OTP to the user)
                // await SendOtpToUser(user, otp);

                return "کد موقت ارسال شد";
            }
            catch (Exception ex)
            {
                return $"خطا: {ex.Message}";
            }
        }
    }
}
