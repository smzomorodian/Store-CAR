using Carproject.Model;
using Domain.Model;
using FirebaseAdmin.Auth;
public class Buyer : User
{
    public Buyer() { }

    public Buyer(string name, string age, string nationalcode, string password, string phonenumber, string role, string Email)
        : base(name, age, nationalcode, password, phonenumber, role,Email)
    {
    }




    /// امیرعلی اضافه کردم
    public List<PurchaseHistory> PurchaseHistories { get; set; } // تاریخچه خرید
    public LoyaltyStatus LoyaltyStatus { get; set; } // وضعیت وفاداری مشتری
    public decimal Points { get; set; } // امتیاز مشتری


    // 🔹 فیلد جدید برای ذخیره علاقه مشتری به دسته‌بندی خودرو با ارتباط چند به چند
    public List<CustomerCategory> InterestedCategories { get; set; } = new();
    // مثال: "SUV" یا "Sedan"

    //مدل سیل
    public ICollection<Sale> Sales { get; set; }

}
