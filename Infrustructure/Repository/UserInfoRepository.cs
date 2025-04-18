using Domain.Model.UserModel;
using Infrustructure.Context;
using Infrustructure.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrustructure.Repository
{
    public class UserInfoRepository<T> : IUserInfoRepository<T>  where T : class , IUserInformation
    {
        private readonly CARdbcontext _context;
        public UserInfoRepository(CARdbcontext context)
        {
            _context = context;
        }
        public async Task<T> getnationalcode(string nationalcode)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(x => x.nationalcode == nationalcode);
        }

        public async Task<T> getpassword(string password)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(x => x.password == password);
        }

        public async Task<T> getphonenmber(string phonenumber)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(x => x.phonenumber == phonenumber);
        }

        public async Task<Buyer?> GetByIdAsync(Guid Id)
        {
            return await _context.buyers
                .FirstOrDefaultAsync(b => b.Id == Id);
        }

    }
}
