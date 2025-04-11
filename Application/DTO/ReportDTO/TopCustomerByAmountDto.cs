using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.ReportDTO
{
    public class TopCustomerByAmountDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string nationalCode { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
