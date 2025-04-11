using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.ReportDTO;
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

       // (مشتریان وفادار(پرتکرارترین خریداران
        public async Task<List<LoyalCustomerDto>> GetLoyalCustomersAsync()
        {
            var result = await _context.Sales
                .Include(s => s.Buyer)
                .GroupBy(s => new { s.BuyerId, s.Buyer.Name, s.Buyer.Email , s.Buyer.nationalcode})
                .Select(g => new LoyalCustomerDto
                {
                    Email = g.Key.Email,
                    Name = g.Key.Name,
                    nationalcode = g.Key.nationalcode,
                    TotalPurchases = g.Count()
                })
                .OrderByDescending(x => x.TotalPurchases)
                .ToListAsync();

            return result;
        }

        /// مشتریان با بیشترین مبلغ کل خرید 
        public async Task<List<TopCustomerByAmountDto>> GetTopCustomersByAmountAsync()
        {
            var result = await _context.Sales
                .Include(s => s.Buyer)
                .GroupBy(s => new { s.BuyerId, s.Buyer.Name, s.Buyer.Email })
                .Select(g => new TopCustomerByAmountDto
                {
                    Name = g.Key.Name,
                    Email = g.Key.Email,
                    TotalAmount = g.Sum(s => s.Amount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToListAsync();

            return result;
        }


        ////  مشتریان جدید تو یکماه اخیر
        public async Task<List<NewCustomerDto>> GetNewCustomersAsync(int days = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);

            var result = await _context.Sales
                .Include(s => s.Buyer)
                .Where(s => s.SaleDate.Date > cutoffDate.Date) // تبدیل تاریخ‌ها به تاریخ بدون زمان
                .GroupBy(s => new { s.BuyerId, s.Buyer.Name, s.Buyer.Email })
                .Select(g => new
                {
                    g.Key.Name,
                    g.Key.Email,
                    FirstPurchaseDate = g.Min(x => x.SaleDate)
                })
                

                .Select(x => new NewCustomerDto
                {
                    Name = x.Name,
                    Email = x.Email,
                    FirstPurchaseDate = x.FirstPurchaseDate
                })
                .OrderByDescending(x => x.FirstPurchaseDate)
                .ToListAsync();

            return result;
        }

    }
}
