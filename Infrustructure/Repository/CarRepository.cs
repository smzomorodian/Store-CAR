using Domain.Model;
using Infrustructure.Context;
using Infrustructure.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrustructure.Repository
{
    public class CarRepository : ICarRepository
    {
        private readonly CARdbcontext _context;

        public CarRepository(CARdbcontext context)
        {
            _context = context;
        }

        public async Task<List<Car>> GetFilteredCarsAsync(
            string brand,
            string model,
            int? minYear,
            int? maxYear,
            decimal? minPrice,
            decimal? maxPrice,
            string color,
            Car.CarStatus? status)
        {
            var query = _context.Cars.AsQueryable();

            if (!string.IsNullOrWhiteSpace(brand))
            {
                query = query.Where(c => c.Brand.Contains(brand));
            }
            if (!string.IsNullOrWhiteSpace(model))
            {
                query = query.Where(c => c.Model.Contains(model));
            }
            if (minYear.HasValue)
            {
                query = query.Where(c => c.Year >= minYear.Value);
            }
            if (maxYear.HasValue)
            {
                query = query.Where(c => c.Year <= maxYear.Value);
            }
            if (minPrice.HasValue)
            {
                query = query.Where(c => c.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.Price <= maxPrice.Value);
            }
            if (!string.IsNullOrWhiteSpace(color))
            {
                query = query.Where(c => c.Color.Contains(color));
            }
            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<List<Car>> GetAllCarsAsync()
        {
            return await _context.Cars.ToListAsync();
        }

        public async Task<Car> GetCarByIdAsync(Guid carId)
        {
            return await _context.Cars.FindAsync(carId);
        }

        //public Task<Car> AddCarAsync(Car car)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<Car> UpdateCarAsync(Car car)
        {
             _context.Cars.Update(car);
            await _context.SaveChangesAsync();
            return car;
        }

        public Task<bool> DeleteCarAsync(Guid carId)
        {
            throw new NotImplementedException();
        }
    }

}
