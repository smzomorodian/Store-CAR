using Application.Command.User.Command;
using Domain.Model;
using FirebaseAdmin.Auth;
using Infrustructure.Repository.IRepository;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Usermodel = Domain.Model.User;

namespace Application.Command.User.CommandHandler
{
    public class UserLoginCommandHandler<T> : IRequestHandler<UserLoginCommand<T>, string> where T : Usermodel, new()
    {
        private readonly IUserInfoRepository<T> _userInfoRepository;
        private string _secretKey;

        public UserLoginCommandHandler(IUserInfoRepository<T> userInfoRepository,IConfiguration configuration)
        {
            _userInfoRepository = userInfoRepository;
            _secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public async Task<string> Handle(UserLoginCommand<T> request, CancellationToken cancellationToken)
        {
            // پیدا کردن کاربر بر اساس کد ملی
            var user = await _userInfoRepository.getnationalcode(request.Nationalcode);
            if (user == null)
            {
                throw new Exception("کاربر یافت نشد");
            }

            // اعتبارسنجی رمز عبور
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.password))
            {
                throw new Exception("نام کاربری یا رمز عبور اشتباه است");
            }

            // تولید توکن JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "http://localhost:5260",
                Audience = "http://localhost:5260",
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

            //var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes("THIS IS USED TO SIGN AND VERIFY JWT TOKENS, REPLACE IT WITH YOUR OWN SECRET");
            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.ToString()) }),
            //    Expires = DateTime.UtcNow.AddHours(1),
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            //    Issuer = "http://localhost:5260",
            //    Audience = "http://localhost:5260"
            //};
            //var token = tokenHandler.CreateToken(tokenDescriptor);
            //return tokenHandler.WriteToken(token);




        }

    }

   }