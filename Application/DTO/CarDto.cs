
using static Domain.Model.Car;

namespace Application.DTO
{
    public class CarDto
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public string VIN { get; set; }
        public string ImagePath { get; set; }

        // وضعیت خودرو
        public CarStatus Status { get; set; }

        // دسته‌بندی
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
    }
}
