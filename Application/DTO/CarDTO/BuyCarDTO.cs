using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.CarDTO
{
    public class BuyCarDTO
    {
        public Guid CarId { get; set; }  // شناسه خودرو
        public Guid BuyerId { get; set; } // شناسه خریدار
    }
}
