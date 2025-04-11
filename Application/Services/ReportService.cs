using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Infrustructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ReportService:IReportService
    {
        private readonly CARdbcontext _context;

        public ReportService(CARdbcontext context)
        {
            _context = context;
        }
        // سرویس گزارش محبوب ترین مدل خودرو

        public async Task<List<PopularCarModelDto>> GetPopularCarModelsAsync()
        {
            var result = await _context.Sales
                .Include(s => s.Car)
                .GroupBy(s => s.Car.Model)
                .Select(g => new PopularCarModelDto
                {
                    CarModel = g.Key,
                    SalesCount = g.Count()
                })
                .OrderByDescending(x => x.SalesCount)
                .ToListAsync();

            return result;
        }

    }
}
