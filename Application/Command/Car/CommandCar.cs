using static Domain.Model.Car;

namespace Application.Command.Car
{
    public class CommandCar
    {
        public CommandCar(string brand, string model, int year, string color, decimal price, string vin, List<FileBaseDto> files, int categoryId, CarStatus? status = null, string name = null)
        {
            Brand = brand;
            Model = model;
            Year = year;
            Color = color;
            Price = price;
            VIN = vin;
            Files = files ?? new List<FileBaseDto>(); // اگر فایل‌ها ارسال نشوند، لیست خالی ایجاد می‌شود
            CategoryId = categoryId;
            Status = status ?? CarStatus.New;
            Name = name;
        }

        public string Name { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public string VIN { get; set; }
        public List<FileBaseDto> Files { get; set; } // لیست فایل‌ها
        public CarStatus? Status { get; set; }
        public int CategoryId { get; set; }

        public CommandCar()
        {

        }
    }

    // DTO برای فایل‌ها (این فقط برای انتقال داده‌ها استفاده می‌شود)

    public class FileBaseDto
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }


}

