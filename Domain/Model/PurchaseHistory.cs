namespace Carproject.Model
{
    public class PurchaseHistory
    {
        public PurchaseHistory(int customerId, DateTime purchaseDate, decimal purchaseAmount, string productName)
        {
            CustomerId = customerId;
            PurchaseDate = purchaseDate;
            PurchaseAmount = purchaseAmount;
            ProductName = productName;
        }

        public int PurchaseHistoryId { get; set; }
        public int CustomerId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchaseAmount { get; set; } // مبلغ خرید
        public string ProductName { get; set; }
    }
}
