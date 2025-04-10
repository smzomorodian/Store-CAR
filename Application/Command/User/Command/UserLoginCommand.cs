using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.User.Command
{
    public class UserLoginCommand<T> : IRequest<string> where T : class
    {
        [Required]
        public string Password { get; set; }
        [Required]
        public string Nationalcode { get; set; }

        public UserLoginCommand(string password, string nationalcode)
        {
            Password = password;
            Nationalcode = nationalcode;
        }

        public UserLoginCommand() { }
    }
}
