using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.User.Command
{
    public class RequestPasswordResetCommand<T> : IRequest<string> where T : class
    {
        public string nationalCode { get; set; }

        public RequestPasswordResetCommand(string nationalCode)
        {
            this.nationalCode = nationalCode;
        }

        public RequestPasswordResetCommand() { }
    }
}
