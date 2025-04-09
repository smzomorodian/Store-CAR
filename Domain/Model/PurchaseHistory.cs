namespace Carproject.Model
{
    public class PurchaseHistory
    {
        public PurchaseHistory(Guid customerId, DateTime purchaseDate, decimal purchaseAmount, string productName)
        {
            CustomerId = customerId;
            PurchaseDate = purchaseDate;
            PurchaseAmount = purchaseAmount;
            ProductName = productName;
        }

        public Guid PurchaseHistoryId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchaseAmount { get; set; } // مبلغ خرید
        public string ProductName { get; set; }
    }
}
