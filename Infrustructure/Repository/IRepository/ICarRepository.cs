using Domain.Model.CarModel;
using Microsoft.EntityFrameworkCore;
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
            decimal? minPrice, decimal? maxPrice, string color, Car.CarStatus? status, Guid? categoryId);
        Task AddCarAsync(Car car);
        Task<Car> UpdateCarAsync(Car car);  // به‌روزرسانی اطلاعات خودرو
        Task<bool> DeleteCarAsync(Guid carId);  // حذف خودرو
        Task SaveAsync();
        Task<bool> IsCategoryValidAsync(Guid categoryId);
        Task<CarCategory> GetDefaultCategoryAsync();

    }
}
