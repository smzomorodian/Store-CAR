
namespace Carproject.DTO
{
    public class PurchaseHistoryDto
    {
        public int CustomerId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchaseAmount { get; set; }
        public string ProductName { get; set; }
    }
}