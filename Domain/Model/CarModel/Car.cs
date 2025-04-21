using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using Domain.Model.File;

namespace Domain.Model.CarModel
{
    public class Car
    {
        public void SetStatus(CarStatus status)
        {
            Status = status;
        }

        // کانستراکتور
        public Car(string brand, string model, int year, string color, decimal price, string vin,
            CarStatus status, Guid categoryId, string name , int stock)
        {
            Id = Guid.NewGuid();
            Brand = brand;
            Model = model;
            Year = year;
            Color = color;
            Price = price;
            VIN = vin.ToUpper().Trim();
            Status = status;
            CategoryId = categoryId;
            FilesIds ="";
            Name = name;
            Stock = stock;
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
        public Guid CategoryId { get; private set; }
        public int? Stock { get; private set; }

        [ForeignKey("CategoryId")]
        public CarCategory Category { get; private set; }

        public string? FilesIds { get; private set; }
        public List<string> FilesIdsList
        {
            get => string.IsNullOrWhiteSpace(FilesIds)
        ? new List<string>()
        : FilesIds.Split(',').ToList();

            private set => FilesIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public Car()
        {

        }

        public void AddFileId(Guid fileId)
        {
            if (fileId == Guid.Empty)
                throw new ArgumentException("آیدی فایل نمی‌تواند خالی باشد.");

            var list = FilesIdsList;
            list.Add(fileId.ToString());
            FilesIds = string.Join(",", list);
        }

        public void SetFilesIds(List<string> fileIdsList)
        {
            if (fileIdsList == null || fileIdsList.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("آیدی فایل‌ها نمی‌توانند نال یا خالی باشند.");

            FilesIds = string.Join(",", fileIdsList);  // تبدیل لیست به رشته‌ای که در دیتابیس ذخیره می‌شود.
        }

        public void AddVin(string vin)
        {
            VIN = vin;
        }


    }
}
