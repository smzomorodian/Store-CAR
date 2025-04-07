namespace Carproject.Model
{
    public class Sale
    {
        public Sale(DateTime saleDate, decimal amount, int customerId)
        {
            SaleDate = saleDate;
            Amount = amount;
            CustomerId = customerId;
        }

        public int Id { get; set; }               // شناسه فروش
        public DateTime SaleDate { get; set; }    // تاریخ فروش
        public decimal Amount { get; set; }       // مبلغ فروش
        public int CustomerId { get; set; }       // شناسه مشتری

    }
}
