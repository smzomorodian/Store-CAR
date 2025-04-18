using Application.Command.User.Command;
using Infrustructure.Repository.IRepository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using usermodel = Domain.Model.UserModel.User;
namespace Application.Command.User.CommandHandler
{
    public class RequestOtpCommandHandler<T>(IRepository<T> genericRepository, IUserInfoRepository<T> userInfoRepository) 
        : IRequestHandler<RequestOtpCommand<T>, string> where T : usermodel
    {
        private readonly IRepository<T> _genericRepository = genericRepository;
        private readonly IUserInfoRepository<T> _userInfoRepository = userInfoRepository;

        public async Task<string> Handle(RequestOtpCommand<T> request, CancellationToken cancellationToken)
        {
            var user = await _userInfoRepository.getphonenmber(request.Phonenumber);

            if (user == null)
            {
                return "کاربر یافت نشد";
            }
            // ایجاد OTP و ذخیره در دیتابیس
            var otp = new Random().Next(100000, 999999).ToString();
            user.Otp = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(10); 

            await _genericRepository.SavechangeAsync();

            // اینجا باید OTP را از طریق SMS یا ایمیل ارسال کنید (مثلا با Twilio یا SendGrid)

            return "OTP ارسال شد";
        }
    }
}
