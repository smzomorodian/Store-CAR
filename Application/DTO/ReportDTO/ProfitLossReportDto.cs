namespace Application.DTO.ReportDTO
{
    public class ProfitLossReportDto
    {
        public decimal TotalSales { get; set; }             // مجموع درآمد از فروش‌ها
        public decimal TotalExpenses { get; set; }          // مجموع هزینه‌ها
        public decimal GrossProfit { get; set; }            // سود ناخالص
        public decimal OperatingExpenses { get; set; }      // هزینه‌های عملیاتی
        public decimal NetProfit { get; set; }              // سود خالص
    }
}
