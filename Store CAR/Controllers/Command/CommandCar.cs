using static Domain.Model.Car;

namespace Carproject.Command
{
    public class CommandCar
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public string VIN { get; set; }
        public string ImagePath { get; set; }
        // وضعیت خودرو
        public CarStatus Status { get; set; } = CarStatus.New; // مقدار پیش‌فرض جدید

        // دسته‌بندی
        public int CategoryId { get; set; }
    }
}
