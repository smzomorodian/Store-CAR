using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrustructure.Repository.IRepository
{
    public interface IUserInfoRepository<T> where T : class
    {
        Task<T> getnationalcode(string nationalcode);
        Task<T> getphonenmber(string phonenumber);
        Task<T> getpassword(string password);
    }
}


