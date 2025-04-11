using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.ReportDTO
{
    public class LoyalCustomerDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string nationalcode { get; set; }

        public int TotalPurchases { get; set; }
    }
}
