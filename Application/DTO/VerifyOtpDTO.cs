﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class VerifyOtpDTO
    {
        public string UserType { get; set; }
        public string PhoneNumber { get; set; }
        public string otp { get; set; }
    }
}
