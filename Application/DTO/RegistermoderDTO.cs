using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class RegistermoderDTO
    {
		public string Name { get; set; }
		public string Age { get; set; }
		public string National_Code { get; set; }
        public string Password { get; set; }
        public string Phonenmber { get; set; }
		public string[] Role { get; set; }
	}
}
