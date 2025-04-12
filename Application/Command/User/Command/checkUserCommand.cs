using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.User.Command
{
    public class checkUserCommand<T> : IRequest<bool> where T : class
    {
        public string nationalcode { get; set; }
        public string UserType { get; set; }

        public checkUserCommand(string nationalcode, string userType)
        {
            this.nationalcode = nationalcode;
            UserType = userType;
        }
        public checkUserCommand() { }
    }
}
