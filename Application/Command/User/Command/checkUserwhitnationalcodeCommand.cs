using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.User.Command
{
    public class checkUserwhitnationalcodeCommand<T> : IRequest<bool> where T : class
    {
        public string nationalcode { get; set; }
        public string UserType { get; set; }

        public checkUserwhitnationalcodeCommand(string nationalcode, string userType)
        {
            this.nationalcode = nationalcode;
            UserType = userType;
        }
        public checkUserwhitnationalcodeCommand() { }
    }
}
