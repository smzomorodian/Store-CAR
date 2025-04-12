using Domain.Model;

namespace Carproject.Model
{
    public class BuyerCategory
    {
        public BuyerCategory(Guid buyerId, Buyer customer, int categoryId, CarCategory category)
        {
            BuyerId = buyerId;
            Buyer = Buyer;
            CategoryId = categoryId;
            Category = category;
        }
        public BuyerCategory()
        {
            
        }
        public int id { get; set; }
        public Guid BuyerId { get; set; }
        public Buyer? Buyer { get; set; }

        public int CategoryId { get; set; }
        public CarCategory Category { get; set; }
    }
}
