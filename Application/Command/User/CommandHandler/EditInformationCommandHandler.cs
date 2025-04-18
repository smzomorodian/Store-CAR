using Application.Command.User.Command;
using Application.DTO;
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
    public class EditInformationCommandHandler<T> : IRequestHandler<EditInformationCommand<T>, string> where T : Usermodel
    {
        private readonly IRepository<T> _genericRepository;
        private readonly IUserInfoRepository<T> _userInfoRepository;

        public EditInformationCommandHandler(IRepository<T> genericRepository, IUserInfoRepository<T> userInfoRepository)
        {
            _genericRepository = genericRepository;
            _userInfoRepository = userInfoRepository;
        }
        public async Task<string> Handle(EditInformationCommand<T> request, CancellationToken cancellationToken)
        {
            var user = await _userInfoRepository.getnationalcode(request.National_Code);
            if (user == null)
            {
                return "User Not Found";
            }

            user.Name = request.Name;
            user.phonenumber = request.Phonenmber;
            user.nationalcode = request.National_Code;
            user.Age = request.Age;
            user.password = request.Password;
            user.Email = request.Email;

            await _genericRepository.SavechangeAsync();
            return ("succesfully Edit ");
        }
    }
}
