namespace Domain.Model.ReportNotifModel
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

        public Guid PurchaseHistoryId { get; private set; }
        public Guid CustomerId { get; private set; }
        public DateTime PurchaseDate { get; private set; }
        public decimal PurchaseAmount { get; private set; } // مبلغ خرید
        public string ProductName { get; private set; }
    }
}
