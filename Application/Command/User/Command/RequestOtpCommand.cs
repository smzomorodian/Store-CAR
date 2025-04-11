using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.User.Command
{
    class RequestOtpCommand : IRequest<string>
    {
        public string Phonenumber { get; set; }
    }
}
