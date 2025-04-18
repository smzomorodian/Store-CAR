namespace Domain.Model.ReportNotifModel
{
    public class Expense
    {
        public Expense(DateTime date, decimal amount, string category)
        {
            Id = Guid.NewGuid();
            Date = date;
            Amount = amount;
            Category = category;
        }

        public Guid Id { get; private set; }               // شناسه هزینه
        public DateTime Date { get; private set; }        // تاریخ هزینه
        public decimal Amount { get; private set; }       // مبلغ هزینه
        public string Category { get; private set; }      // دسته‌بندی هزینه (اختیاری: مواد اولیه، حقوق، غیره)
    }
}
