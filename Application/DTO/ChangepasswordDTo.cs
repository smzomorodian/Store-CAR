using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class ChangepasswordDTo
    {
        public string NationalCode { get; set; }
        public string Otp { get; set; }
        public string NewPassword { get; set; }
    }
}
