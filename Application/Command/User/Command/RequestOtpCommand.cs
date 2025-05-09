﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.User.Command
{
    public class RequestOtpCommand<T> : IRequest<string> where T : class
    {
        public string UserType { get; set; }
        public string Phonenumber { get; set; }

        public RequestOtpCommand(string userType, string phonenumber)
        {
            UserType = userType;
            Phonenumber = phonenumber;
        }
        public RequestOtpCommand() { }
    }
}
