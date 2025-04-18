using System;
using Domain.Model.UserModel;

namespace Domain.Model.ReportNotifModel
{
    public class Notification
    {
        // نوتیف برای اطللاع رسانی خودرو جدید
        //public Notification(string title, string message, DateTime createdAt, Guid carId, Guid customerId)
        //{
        //    Id = Guid.NewGuid();
        //    Title = title;
        //    Message = message;
        //    CreatedAt = createdAt;
        //    CarId = carId;
        //    CustomerId = customerId;

        //}

        public Notification(string title, string message, DateTime createdAt, Guid carId, Guid buyerId)
        {
            Id = Guid.NewGuid();
            Title = title;
            Message = message;
            CreatedAt = createdAt;
            CarId = carId;
            BuyerId = buyerId;
        }


        // نوتیف برای فروش جدید 
        public Notification(string title, string message, DateTime createdAt, Guid customerId)
        {
            Id = Guid.NewGuid();
            Title = title;
            Message = message;
            CreatedAt = createdAt;
            BuyerId = customerId;
        }

        public Guid Id { get; private set; }               // شناسه نوتیفیکیشن
        public string Title { get; private set; }         // عنوان نوتیفیکیشن
        public string Message { get; private set; }       // متن پیام
        public DateTime CreatedAt { get; private set; }   // زمان ایجاد
        public bool IsRead { get;private set; }          // آیا خوانده شده است؟
        public User? User { get; set; }    // ارتباط با جدول کاربران
        
        public Guid CarId { get; private set; }            // شناسه خودرو
        public Guid BuyerId { get; private set; }       // شناسه مشتری

        public void MarkAsRead()
        {
            IsRead = true;
        }


    }
}
