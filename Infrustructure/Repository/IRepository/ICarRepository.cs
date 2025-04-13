using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Infrustructure.Repository.IRepository
{
    public interface ICarRepository
    {

        Task<List<Car>> GetAllCarsAsync();  // دریافت تمامی خودروها
        Task<Car> GetCarByIdAsync(Guid carId);  // دریافت خودرو بر اساس شناسه
        Task<List<Car>> GetFilteredCarsAsync(string brand, string model, int? minYear, int? maxYear, 
            decimal? minPrice, decimal? maxPrice, string color, Car.CarStatus? status);
        //Task<Car> AddCarAsync(Car car);  // اضافه کردن خودرو جدید
        Task<Car> UpdateCarAsync(Car car);  // به‌روزرسانی اطلاعات خودرو
        Task<bool> DeleteCarAsync(Guid carId);  // حذف خودرو
    }
}
