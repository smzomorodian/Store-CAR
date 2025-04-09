using Domain.Model;

namespace Carproject.Model
{
    public class CustomerCategory
    {
        public CustomerCategory(int customerId, Customer customer, int categoryId, CarCategory category)
        {
            CustomerId = customerId;
            Customer = customer;
            CategoryId = categoryId;
            Category = category;
        }
        public CustomerCategory()
        {
            
        }
        public int id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int CategoryId { get; set; }
        public CarCategory Category { get; set; }
    }
}
