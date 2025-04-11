using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.User.Command
{
    public class ChangePasswordcommand<T> :IRequest<String> where T :class
    {
        public string NationalCode { get; set; }
        public string Otp { get; set; }
        public string NewPassword { get; set; }

        public ChangePasswordcommand(string nationalCode, string otp, string newPassword)
        {
            NationalCode = nationalCode;
            Otp = otp;
            NewPassword = newPassword;
        }

        public ChangePasswordcommand() { }
    }
}
