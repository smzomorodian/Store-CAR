using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class User
    {
        public User() { }

        public User(string name, string age, string national_Code,string password, string phonenumber ,string[] role, string otp) 
        {
            Id = Guid.NewGuid();
            Name = name;
            Age = age;
            National_Code = national_Code;
            Password = password;
            Phonenmber = phonenumber;
            Role = role;
            Otp = otp;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public string National_Code { get; set; }
        public string Password { get; set; }
        public string Phonenmber { get; set; }
        public string[] Role { get; set; }
        public string Otp { get; set; } // کد موقت برای بازنشانی رمز عبور
        public DateTime? OtpExpiry { get; set; } // تاریخ انقضا کد یک بار مصرف
    }
}
