﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.UserModel
{
    public class Seller : User
    {
        public Seller() : base() { }
        public Seller(string name, string age, string nationalcode, string password, string phonenumber, string role, string email)
            : base(name, age, nationalcode, password, phonenumber, role, email)
        {

        }
    }
}
