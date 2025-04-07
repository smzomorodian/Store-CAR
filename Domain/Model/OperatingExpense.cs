namespace Carproject.Model
{
    public class OperatingExpense
    {
        public int Id { get; set; }               // شناسه هزینه عملیاتی
        public DateTime Date { get; set; }        // تاریخ هزینه عملیاتی
        public decimal Amount { get; set; }       // مبلغ هزینه عملیاتی
        public string Description { get; set; }   // توضیحات مربوط به هزینه عملیاتی
    }
}
