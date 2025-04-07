namespace Carproject.Model
{
    public class Notification
    {
        // نوتیف برای اطللاع رسانی خودرو جدید
        public Notification(string title, string message, DateTime createdAt, int carId, int customerId, Customer customer)
        {
            Title = title;
            Message = message;
            CreatedAt = createdAt;
            CarId = carId;
            CustomerId = customerId;
            Customer = customer;
        }

        public Notification(string title, string message, DateTime createdAt, int carId, int customerId)
        {
            Title = title;
            Message = message;
            CreatedAt = createdAt;
            CarId = carId;
            CustomerId = customerId;
        }


        // نوتیف برای فروش جدید 
        public Notification(string title, string message, DateTime createdAt, int customerId)
        {
            Title = title;
            Message = message;
            CreatedAt = createdAt;
            CustomerId = customerId;
        }

        public int Id { get; set; }               // شناسه نوتیفیکیشن
        public string Title { get; set; }         // عنوان نوتیفیکیشن
        public string Message { get; set; }       // متن پیام
        public DateTime CreatedAt { get; set; }   // زمان ایجاد
        public bool IsRead { get; set; }          // آیا خوانده شده است؟
        public Customer Customer { get; set; }            // ارتباط با جدول کاربران
        public int CarId { get; set; }            // شناسه خودرو
        public int CustomerId { get; set; }       // شناسه مشتری
    }
}
