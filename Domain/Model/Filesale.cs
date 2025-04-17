using Carproject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Filesale : FileBaseSale
    {
        public Guid saleId { get; set; }

        public Sale sale { get; set; }

        public Filesale(string filename , string filePath , Guid saleId) : base(filename , filePath , saleId)
        {

        }
    }
}
