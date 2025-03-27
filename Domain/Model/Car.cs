using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Model
{
    public class Car
    {
        public enum CarStatus
        {
            New,
            Used,
            Sold
        }

        [Key]
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public string VIN { get; set; } // شماره شاسی
        public string ImagePath { get; set; } // مسیر تصویر خودرو

        public CarStatus Status { get; set; }  // اینجا وضعیت خودرو را مشخص می‌کنیم

        //دسته‌بندی خودرو
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public CarCategory Category { get; set; }
    }

}
