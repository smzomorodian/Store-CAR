using Domain.Model.CarModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.CarDTO
{
    public class CarFilterDTO
    {
        public string Brand { get; set; }  // برند خودرو
        public string Model { get; set; }  // مدل خودرو
        public int? MinYear { get; set; }  // سال تولید حداقل
        public int? MaxYear { get; set; }  // سال تولید حداکثر
        public decimal? MinPrice { get; set; }  // قیمت حداقل
        public decimal? MaxPrice { get; set; }  // قیمت حداکثر
        public string Color { get; set; }  // رنگ خودرو
        public Car.CarStatus? Status { get; set; }  // وضعیت خودرو
    }
}
