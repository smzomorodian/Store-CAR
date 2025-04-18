using static Domain.Model.CarModel.Car;

namespace Application.DTO.CarDTO
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
        public string Status { get; set; }

        // دسته‌بندی
        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }
    }
}
