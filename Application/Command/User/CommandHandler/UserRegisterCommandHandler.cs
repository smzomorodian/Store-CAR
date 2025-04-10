using Application.Command.User.Command;
using Application.DTO;
using Infrustructure.Repository;
using Infrustructure.Repository.IRepository;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Usermodel = Domain.Model.User;

namespace Application.Command.User.CommandHandler
{
    public class UserRegisterCommandHandler<T> : IRequestHandler<UserRegisterCommand<T>, Guid> where T : Usermodel, new()
    {
        private readonly IRepository<T> _genericRepository;

        public UserRegisterCommandHandler(IRepository<T> repository)
        {
            _genericRepository = repository;
        }

        public async Task<Guid> Handle(UserRegisterCommand<T> request, CancellationToken cancellationToken)
        {
            //var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new T
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Age = request.Age,
                nationalcode = request.National_Code,
                password = request.Password,
                phonenumber = request.Phonenmber,
                Role = request.Role,
                Otp = null
            };

            await _genericRepository.AddAsync(user);
            await _genericRepository.SavechangeAsync();
            return user.Id;
        }
    }
}