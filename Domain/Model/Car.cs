using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carproject.Model;
using System.Drawing;

namespace Domain.Model
{
    public class Car
    {
        public void SetStatus(CarStatus status)
        {
            Status = status;
        }

        // کانستراکتور
        public Car(string brand, string model, int year, string color, decimal price, string vin,
            CarStatus status, int categoryId, string name)
        {
            Id = Guid.NewGuid();
            Brand = brand;
            Model = model;
            Year = year;
            Color = color;
            Price = price;
            VIN = vin;
            Status = status;
            CategoryId = categoryId;
            Files = new List<FileBase>();
            Name = name;
        }
        public enum CarStatus
        {
            New,
            Used,
            Sold
        }

        [Key]
        public Guid Id { get; private set; }

        public string Name { get; private set; }
        public string Brand { get; private set; }
        public string Model { get; private set; }
        public int Year { get; private set; }
        public string Color { get; private set; }
        public decimal Price { get; private set; }
        public string VIN { get; private set; } // شماره شاسی
        public CarStatus Status { get; private set; }
        public int CategoryId { get; private set; }

        [ForeignKey("CategoryId")]
        public CarCategory Category { get; private set; }

        // اضافه کردن فیلد Files
        public List<FileBase> Files { get; private set; } = new List<FileBase>(); // فرض بر این است که FileBase کلاسی از فایل‌ها باشد.

        public Car()
        {

        }
    }
}
