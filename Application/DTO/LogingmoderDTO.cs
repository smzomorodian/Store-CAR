﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class LogingmoderDTO
    {
		[Required]
		public string Name { get; set; }
		[Required]
		public string Password { get; set; }

	}
}
