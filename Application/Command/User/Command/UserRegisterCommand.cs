using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.User.Command
{
    public class UserRegisterCommand<T> : IRequest<Guid> where T:class
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string National_Code { get; set; }
        public string Password { get; set; }
        public string Phonenmber { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public UserRegisterCommand(string name, string age, string national_Code, string password, string phonenmber, string email ,string role)
        {
            Name = name;
            Age = age;
            National_Code = national_Code;
            Password = password;
            Phonenmber = phonenmber;
            Email = email;
            Role = role;
        }

        public UserRegisterCommand() { }
    }
}
