using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.User.Command
{
    public class VerifyOtpCommand<T> : IRequest<string> where T : class
    {
        public string UserType { get; set; }
        public string PhoneNumber { get; set; }
        public string otp { get; set; }


        public VerifyOtpCommand(string userType, string phoneNumber, string otp)
        {
            UserType = userType;
            PhoneNumber = phoneNumber;
            this.otp = otp;
        }

        public VerifyOtpCommand() { }
    }
}
