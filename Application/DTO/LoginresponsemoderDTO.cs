﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model.UserModel;

namespace Application.DTO
{
    public class LoginresponsemoderDTO
    {
       public Moder moder { get; set; } 
       public string token { get; set; }
    }
}
