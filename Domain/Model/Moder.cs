using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Domain.Model
{
    public class Moder : User
    {
        public Moder() : base() { }
        public Moder(string name , string age , string nationalcode , string password, string phonenumber , string role,string email) 
            : base( name,  age, nationalcode,  password,  phonenumber,  role, email)
        {

        }
    }
}
