using Application.DTO.ReportDTO;
using Application.Services;
using AutoMapper;
using Infrustructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Carproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly CARdbcontext  _context;
        private readonly IMapper _mapper;
        private readonly IReportService _reportService;

        public ReportController(CARdbcontext context, IMapper mapper, IReportService reportService)
        {
            _context = context;
            _mapper = mapper;
            _reportService = reportService;
        }

        //// دریافت گزارش فروش

        [HttpGet("sales-report")]
        public async Task<ActionResult<IEnumerable<SalesReportDto>>> GetSalesReport(DateTime startDate, DateTime endDate)
        {
            var salesReport = await _reportService.GetSalesReportAsync(startDate, endDate);

            if (salesReport == null || !salesReport.Any())
            {
                return NotFound("No sales found for the given date range.");
            }

            return Ok(salesReport);
        }

        ///محاسبه و گزارش سود و ضرر 

        [HttpGet("profit-loss-report")]
        public async Task<ActionResult<ProfitLossReportDto>> GetProfitLossReport(DateTime startDate, DateTime endDate)
        {
            var report = await _reportService.GetProfitLossReportAsync(startDate, endDate);

            return Ok(report);
        }

        /// محاسبه جمع خرید

        [HttpGet("total-sales")]
        public async Task<ActionResult<decimal>> GetTotalSales()
        {
            var totalSales = await _context.Sales.SumAsync(s => s.Amount);
            return Ok(totalSales);
        }

        // گزارش محبوبترین مدل خودرو
        [HttpGet("popular-car-models")]
        public async Task<IActionResult> GetPopularCarModels()
        {
            var result = await _reportService.GetPopularCarModelsAsync();
            return Ok(result);
        }

        //مشتریان وفادار (پرتکرارترین خریداران).
        [HttpGet("loyal-customers")]
        public async Task<IActionResult> GetLoyalCustomers()
        {
            var result = await _reportService.GetLoyalCustomersAsync();
            return Ok(result);
        }

        // مشتریان با بیشترین مبلغ کل خرید
        [HttpGet("top-customers-by-amount")]
        public async Task<IActionResult> GetTopCustomersByAmount()
        {
            var result = await _reportService.GetTopCustomersByAmountAsync();
            return Ok(result);
        }

        //  مشتریان جدید تو یکماه اخیر
        [HttpGet("new-customers")]
        public async Task<IActionResult> GetNewCustomers([FromQuery] int days = 30)
        {
            var result = await _reportService.GetNewCustomersAsync(days);
            return Ok(result);
        }


    }
}





//[HttpGet("sales-report")]
//public async Task<ActionResult<IEnumerable<SalesReportDto>>> GetSalesReport(DateTime startDate, DateTime endDate)
//{
//    // فیلتر کردن فروش‌ها در بازه زمانی مشخص شده
//    var sales = await _context.PurchaseHistories
//        .Where(p => p.PurchaseDate >= startDate && p.PurchaseDate <= endDate)
//        .ToListAsync();

//    if (sales == null || sales.Count == 0)
//    {
//        return NotFound("No sales found for the given date range.");
//    }

//    // گروه‌بندی فروش‌ها بر اساس تاریخ خرید
//    var salesReport = await _context.PurchaseHistories
//                    .Where(p => p.PurchaseDate >= startDate && p.PurchaseDate <= endDate)
//                    .GroupBy(p => p.PurchaseDate.Date)
//                    .Select(g => new SalesReportDto
//                    {
//                        Date = g.Key,
//                        TotalSales = g.Sum(s => s.PurchaseAmount),
//                        NumberOfSales = g.Count()
//                    }).ToListAsync();

//    return Ok(salesReport);
//}




//[HttpGet("profit-loss-report")]
//public async Task<ActionResult<ProfitLossReportDto>> GetProfitLossReport(DateTime startDate, DateTime endDate)
//{
//    // محاسبه درآمدها (فروش‌ها)
//    var sales = await _context.Sales
//        .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
//        .SumAsync(s => s.Amount);

//    // محاسبه هزینه‌ها
//    var expenses = await _context.Expenses
//        .Where(e => e.Date >= startDate && e.Date <= endDate)
//        .SumAsync(e => e.Amount);

//    // محاسبه سود ناخالص و سود خالص
//    var grossProfit = sales - expenses; // فرض بر این است که همه هزینه‌ها مربوط به تولید و فروش است

//    // ممکن است هزینه‌های عملیاتی و مالیات هم نیاز باشد
//    var operatingExpenses = await _context.OperatingExpenses
//        .Where(o => o.Date >= startDate && o.Date <= endDate)
//        .SumAsync(o => o.Amount);

//    var netProfit = grossProfit - operatingExpenses;

//    // ساخت گزارش و ارسال پاسخ
//    var report = new ProfitLossReportDto
//    {
//        TotalSales = sales,
//        TotalExpenses = expenses,
//        GrossProfit = grossProfit,
//        OperatingExpenses = operatingExpenses,
//        NetProfit = netProfit
//    };

//    return Ok(report);
//}
