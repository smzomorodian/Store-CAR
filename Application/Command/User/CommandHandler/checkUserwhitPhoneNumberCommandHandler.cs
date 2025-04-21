using Application.Command.User.Command;
using Infrustructure.Repository.IRepository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserModel = Domain.Model.UserModel.User;
namespace Application.Command.User.CommandHandler
{
    public class checkUserwhitPhoneNumberCommandHandler<T> : IRequestHandler<checkUserwhitPhoneNumberCommand<T>, bool> where T : UserModel
    {
        private readonly IRepository<T> _genericRepository;
        private readonly IUserInfoRepository<T> _userInfoRepository;

        public checkUserwhitPhoneNumberCommandHandler(IRepository<T> genericRepository, IUserInfoRepository<T> userInfoRepository)
        {
            _genericRepository = genericRepository;
            _userInfoRepository = userInfoRepository;
        }
        public async Task<bool> Handle(checkUserwhitPhoneNumberCommand<T> request, CancellationToken cancellationToken)
        {
            var user = await _userInfoRepository.getphonenmber(request.Phonenumber);
            if (user == null)
            {
                return true;
            }
            return false;
        }
    }
}
