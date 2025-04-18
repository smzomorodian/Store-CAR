namespace Domain.Model.ReportNotifModel
{
    public class OperatingExpense
    {
        public Guid Id { get; private set; }               // شناسه هزینه عملیاتی
        public DateTime Date { get; private set; }        // تاریخ هزینه عملیاتی
        public decimal Amount { get; private set; }       // مبلغ هزینه عملیاتی
        public string Description { get; private set; }   // توضیحات مربوط به هزینه عملیاتی
    }
}
