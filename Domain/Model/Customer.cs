using System.ComponentModel.DataAnnotations;

namespace Carproject.Model
{
    public class Customer
    {
        public Customer(string firstName, string lastName, string email, string phoneNumber, DateTime dateOfBirth, List<CustomerCategory> interestedCategories)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            DateOfBirth = dateOfBirth;
            InterestedCategories = interestedCategories;
        }
        public Customer()
        {
            
        }

        [Key]
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<PurchaseHistory> PurchaseHistories { get; set; } // تاریخچه خرید
        public LoyaltyStatus LoyaltyStatus { get; set; } // وضعیت وفاداری مشتری
        public decimal Points { get; set; } // امتیاز مشتری

            
        // 🔹 فیلد جدید برای ذخیره علاقه مشتری به دسته‌بندی خودرو با ارتباط چند به چند
        public List<CustomerCategory> InterestedCategories { get; set; } = new();
        // مثال: "SUV" یا "Sedan"
    }
}
