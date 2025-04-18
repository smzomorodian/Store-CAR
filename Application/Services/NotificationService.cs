using Infrustructure.Context;

namespace Carproject.Services
{
    public class NotificationService
    {
        //نوتیف سرویس قبلی
        private readonly CARdbcontext _context;

        public NotificationService(CARdbcontext context)
        {
            _context = context;
        }

        //public async Task SendNotificationAsync(int customerId, string title, string message)
        //{
        //    //var notification = new Notification
        //    //{
        //    //    CustomerId = customerId,
        //    //    Title = title,
        //    //    Message = message,
        //    //    CreatedAt = DateTime.UtcNow,
        //    //    IsRead = false
        //    //};
        //    var notification = new Notification
        //    (
        //        customerId,
        //        title,
        //        message,
        //        DateTime.UtcNow,
        //        false
        //    );

        //    _context.Notifications.Add(notification);
        //    await _context.SaveChangesAsync();
        //}







        //private readonly EmailService _emailService;

        //public NotificationService( EmailService emailService)
        //{

        //    _emailService = emailService;
        //}

        //public async Task SendPurchaseConfirmationAsync(int userId, string phoneNumber, string email)
        //{
        //    string message = "خرید شما تایید شد. با تشکر از خریدتان.";

        //    await _emailService.SendEmailAsync(email, "تایید خرید", message);
        //}

        //public async Task NotifyNewCarAsync(string email)
        //{
        //    string message = "خودروی جدیدی به فروشگاه اضافه شد. برای اطلاعات بیشتر به سایت مراجعه کنید.";
        //    _emailService.SendEmailAsync(email, "خودروی جدید وارد فروشگاه شد", message);
        //}
    }
}
