using Domain.Model.CarModel;

namespace Domain.Model.UserModel
{
    public class BuyerCategory
    {
        public BuyerCategory(Guid buyerId, Guid categoryId)
        {
            id = Guid.NewGuid();
            BuyerId = buyerId;
            CategoryId = categoryId;
        }
        public BuyerCategory()
        {

        }
        public Guid id { get; private set; }
        public Guid BuyerId { get; private set; }
        public Buyer? Buyer { get;  set; }

        public Guid CategoryId { get; private set; }
        public CarCategory Category { get;  set; }
    }
}
