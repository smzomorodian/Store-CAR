using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class FileBaseSale
    {
        public FileBaseSale()
        {
        }

        public FileBaseSale(string fileName, string filePath, Guid saleId)
        {
            FileName = fileName;
            FilePath = filePath;
            SaleId = saleId;

        }

        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Guid SaleId { get; set; }

        //[ForeignKey("CarId")]
        //public Car Car { get; set; }
    }
}

