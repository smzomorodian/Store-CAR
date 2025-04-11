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

        // دریافت گزارش فروش
        public async Task<IEnumerable<SalesReportDto>> GetSalesReportAsync(DateTime startDate, DateTime endDate)
        {
            var salesReport = await _context.PurchaseHistories
                .Where(p => p.PurchaseDate >= startDate && p.PurchaseDate <= endDate)
                .GroupBy(p => p.PurchaseDate.Date)
                .Select(g => new SalesReportDto
                {
                    Date = g.Key,
                    TotalSales = g.Sum(s => s.PurchaseAmount),
                    NumberOfSales = g.Count()
                })
                .ToListAsync();

            return salesReport;
        }

        ///محاسبه و گزارش سود و ضرر 
        public async Task<ProfitLossReportDto> GetProfitLossReportAsync(DateTime startDate, DateTime endDate)
        {
            // محاسبه درآمدها (فروش‌ها)
            var sales = await _context.Sales
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .SumAsync(s => s.Amount);

            // محاسبه هزینه‌ها
            var expenses = await _context.Expenses
                .Where(e => e.Date >= startDate && e.Date <= endDate)
                .SumAsync(e => e.Amount);

            // محاسبه سود ناخالص و سود خالص
            var grossProfit = sales - expenses;

            // هزینه‌های عملیاتی
            var operatingExpenses = await _context.OperatingExpenses
                .Where(o => o.Date >= startDate && o.Date <= endDate)
                .SumAsync(o => o.Amount);

            var netProfit = grossProfit - operatingExpenses;

            // ساخت گزارش و ارسال پاسخ
            var report = new ProfitLossReportDto
            {
                TotalSales = sales,
                TotalExpenses = expenses,
                GrossProfit = grossProfit,
                OperatingExpenses = operatingExpenses,
                NetProfit = netProfit
            };

            return report;
        }























    }


}

