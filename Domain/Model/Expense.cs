namespace Carproject.Model
{
    public class Expense
    {
        public Expense(int id, DateTime date, decimal amount, string category)
        {
            Id = id;
            Date = date;
            Amount = amount;
            Category = category;
        }

        public int Id { get; set; }               // شناسه هزینه
        public DateTime Date { get; set; }        // تاریخ هزینه
        public decimal Amount { get; set; }       // مبلغ هزینه
        public string Category { get; set; }      // دسته‌بندی هزینه (اختیاری: مواد اولیه، حقوق، غیره)
    }
}
