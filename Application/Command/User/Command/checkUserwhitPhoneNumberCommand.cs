using Infrustructure.Repository.IRepository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.User.Command
{
    public class checkUserwhitPhoneNumberCommand<T> : IRequest<bool> where T : class
    {
        public string Phonenumber { get; set; }
        public string UserType { get; set; }

        public checkUserwhitPhoneNumberCommand(string phonenumber, string userType)
        {
            Phonenumber = phonenumber;
            UserType = userType;
        }

        public checkUserwhitPhoneNumberCommand() { }
    }
}
