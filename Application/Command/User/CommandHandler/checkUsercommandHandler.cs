using Application.Command.User.Command;
using Infrustructure.Repository.IRepository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usermodel = Domain.Model.UserModel.User;
namespace Application.Command.User.CommandHandler
{
    public class checkUsercommandHandler<T> : IRequestHandler<checkUserCommand<T>, bool> where T : Usermodel
    {
        private readonly IRepository<T> _genericRepository;
        private readonly IUserInfoRepository<T> _userInfoRepository;

        public checkUsercommandHandler(IRepository<T> genericRepository, IUserInfoRepository<T> userInfoRepository)
        {
            _genericRepository = genericRepository;
            _userInfoRepository = userInfoRepository;
        }
        public async Task<bool> Handle(checkUserCommand<T> request, CancellationToken cancellationToken)
        {
            var user = await _userInfoRepository.getnationalcode(request.nationalcode);
            if (user == null)
            {
                return false ;

            }
            return  true;
        }
    }
}
