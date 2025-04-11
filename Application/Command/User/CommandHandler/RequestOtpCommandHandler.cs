using Application.Command.User.Command;
using Infrustructure.Repository.IRepository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.User.CommandHandler
{
    class RequestOtpCommandHandler : IRequestHandler<RequestOtpCommand, string>
    {
        private readonly IRepository<T> _genericRepository;
        private readonly IUserInfoRepository<T> _userInfoRepository;

        public ChangePasswordcommandHandler(IRepository<T> genericRepository, IUserInfoRepository<T> userInfoRepository)
        {
            _genericRepository = genericRepository;
            _userInfoRepository = userInfoRepository;
        }
        public Task<string> Handle(RequestOtpCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
