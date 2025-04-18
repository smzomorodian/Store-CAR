using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.UserModel
{
    public interface IUserInformation
    {
        string nationalcode { get; set; }
        string phonenumber { get; set; }
        string password { get; set; }
    }
}
