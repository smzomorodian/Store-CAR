using FirebaseAdmin.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.UserModel
{
    public abstract class User : IUserInformation
    {
        public User() { }

        public User(string name, string age, string national_Code, string password, string phonenumber, string role, string email)
        {
            Id = Guid.NewGuid();
            Name = name;
            Age = age;
            nationalcode = national_Code;
            this.password = password;
            this.phonenumber = phonenumber;
            Role = role;
            Email = email;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public string nationalcode { get; set; }
        public string password { get; set; }
        public string phonenumber { get; set; }
        public string Role { get; set; }

        public string Email { get; set; }
        public string? Otp { get; set; } // کد موقت برای بازنشانی رمز عبور
        public DateTime? OtpExpiry { get; set; } // تاریخ انقضا کد یک بار مصرف

        //public string phonenumber { get; set; }
        //public string nationalcode { get ; set ; }
    }
}
