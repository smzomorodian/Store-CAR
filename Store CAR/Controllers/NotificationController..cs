using Application.Services;
using AutoMapper;
using Carproject.DTO;
using Carproject.Services;
using Domain.Model.ReportNotifModel;
using Infrustructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Carproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly CARdbcontext _context;
        private readonly IMapper _mapper;
        //private readonly NotificationService _notificationService;
        private readonly ISaleNotificationService _saleNotificationService;

        public NotificationController(CARdbcontext context, IMapper mapper, ISaleNotificationService saleNotificationService)
        {
            _context = context;
            _mapper = mapper;
            //_notificationService = notificationService;
            _saleNotificationService = saleNotificationService;
        }

        // ارسال نوتیف و ایمیل با استفاده از سرویس SaleNotificationService
        [HttpPost("send-sale-notification/{saleId}")]
        public async Task<IActionResult> SendSaleNotification(Guid saleId)
        {
            try
            {
                await _saleNotificationService.SendSaleNotificationAsync(saleId);
                return Ok("نوتیفیکیشن و ایمیل با موفقیت ارسال شد.");
            }
            catch (Exception ex)
            {
                return BadRequest("خطا: " + ex.Message);
            }
        }

        [HttpPost("payment-confirmation/{saleId}")]
        public async Task<IActionResult> SendSaleConfirmationPayNotification(Guid saleId)
        {
            try
            {
                await _saleNotificationService.SendPaymentConfirmationNotificationAsync(saleId);
                return Ok("نوتیفیکیشن و ایمیل با موفقیت ارسال شد.");
            }
            catch (Exception ex)
            {
                return BadRequest("خطا: " + ex.Message);
            }
        }

        // گرفتن نوتیف مشتری

        [HttpGet("customer-notifications/{customerId}")]
        public async Task<ActionResult<List<Notification>>> GetCustomerNotifications(Guid customerId)
        {

            // بررسی وجود مشتری با شناسه داده شده
            var customerExists = await _context.buyers
                .AnyAsync(c => c.Id == customerId);  // بررسی وجود مشتری با این شناسه

            // اگر مشتری با این شناسه وجود نداشت
            if (!customerExists)
            {
                return NotFound($"مشتری با شناسه {customerId} پیدا نشد.");  // خطا 404 بازگردانده می‌شود
            }


            var notifications = await _context.Notifications
                .Where(n => n.BuyerId == customerId)  // تغییر از UserId به CustomerId
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            if (notifications == null || notifications.Count == 0)
                return Ok(new { message = "مشتری هیچ نوتیفیکیشنی ندارد." });

            return Ok(notifications);
        }


        // علامت زدن نوتیف که خوانده شده یا نه
        [HttpPost("mark-as-read/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return NotFound();

            notification.MarkAsRead();
            await _context.SaveChangesAsync();

            return Ok("نوتیفیکیشن به عنوان خوانده‌شده علامت‌گذاری شد.");
        }


        /// اطلاع رسانی خودرو جدید
        [HttpPost("notify-new-car/{carId}")]
        public async Task<IActionResult> NotifyNewCar(Guid carId)
        {
            // بررسی اینکه خودرو در سیستم ثبت شده باشد
            var car = await _context.Cars.FindAsync(carId);
            Console.WriteLine($"Car ID: {car.Id}, CategoryId: {car.CategoryId}");

            if (car == null)
            {
                return NotFound($"خودرو با شناسه {carId} یافت نشد.");
            }
            if (car.CategoryId == null)
            {
                return NotFound($"دسته بندی خودرو {carId} نبود");
            }



            // پیدا کردن مشتریانی که به این دسته‌بندی خودرو علاقه دارند

            var interestedCustomers = await _context.buyers
            .Include(c => c.InterestedCategories)
            .Where(c => c.InterestedCategories.Any(category => category.CategoryId == car.CategoryId))
            .ToListAsync();




            if (!interestedCustomers.Any())
            {
                return Ok("هیچ مشتری علاقه‌مندی برای این خودرو یافت نشد.");
            }

            // ارسال نوتیفیکیشن برای مشتریان علاقه‌مند
            foreach (var customer in interestedCustomers)
            {
                //var notification = new Notification
                //{
                //    CarId = car.Id,
                //    CustomerId = customer.CustomerId,
                //    Title = "خودروی جدید موجود شد!",
                //    Message = $"خودروی {car.Name} با قیمت {car.Price} به فروشگاه اضافه شد.",
                //    CreatedAt = DateTime.Now
                //};
                var notification = new Notification
                (
                    "خودروی جدید موجود شد!",
                    $"خودروی {car.Name} با قیمت {car.Price} به فروشگاه اضافه شد.",
                    DateTime.Now,
                    car.Id,
                    customer.Id
                );

                _context.Notifications.Add(notification);

            }

            await _context.SaveChangesAsync();

            return Ok("نوتیفیکیشن برای مشتریان علاقه‌مند ارسال شد.");
        }



        // 📌 API برای دریافت مشتری و بررسی `InterestedCategories` مشتری علاقمند
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomer(Guid customerId)
        {
            var customer = await _context.buyers
                .Include(c => c.InterestedCategories)
                .ThenInclude(cc => cc.Category)  // برای آوردن اطلاعات دسته‌بندی
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null)
            {
                return NotFound(new { message = "Customer not found" });
            }

            // بررسی مقدار `InterestedCategories`
            bool hasInterestedCategories = customer.InterestedCategories != null && customer.InterestedCategories.Any();

            return Ok(new
            {
                customer.Id,
                customer.Name,

                InterestedCategories = customer.InterestedCategories.Select(cc => new
                {
                    cc.CategoryId,
                    CategoryName = cc.Category.Name
                }),
                hasInterestedCategories
            });
        }





    }
}



//متد ارسال نوتیف با پرکردن فروش
//[HttpPost("add-sale")]
//public async Task<IActionResult> AddSale([FromBody] Sale sale)
//{
//    // بررسی صحت اطلاعات ورودی
//    if (sale == null || sale.Amount <= 0)
//    {
//        return BadRequest("اطلاعات فروش معتبر نیست.");
//    }

//    if (sale.CustomerId == null || sale.CustomerId <= 0)
//    {
//        return BadRequest("CustomerId معتبر نیست.");
//    }

// بررسی وجود CustomerId در دیتابیس
//    var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == sale.CustomerId);
//    if (!customerExists)
//    {
//        return BadRequest("مشتری با این شناسه وجود ندارد.");
//    }

//    //try
//    //{
//        // ثبت فروش در دیتابیس
//        _context.Sales.Add(sale);
//        await _context.SaveChangesAsync();

//        // ارسال نوتیفیکیشن به مدیر
//        await _notificationService.SendNotificationAsync(1, "فروش جدید",
//            $"یک خودرو به مبلغ {sale.Amount:N0} تومان توسط مشتری {sale.CustomerId} در تاریخ {sale.SaleDate:yyyy/MM/dd} فروخته شد.");

//        return Ok(new
//        {
//            Message = "فروش با موفقیت ثبت شد و نوتیفیکیشن ارسال شد.",
//            SaleId = sale.Id,
//            CustomerId = sale.CustomerId,
//            Amount = sale.Amount,
//            SaleDate = sale.SaleDate
//        });
//    //}
//    //catch (Exception ex)
//    //{
//    //    return StatusCode(500, $"خطا در ثبت فروش: {ex.Message}");
//    //}
//}

// این برای ارسال نوتیف بود 

//[HttpPost("send-sale-notification/{saleId}")]
//public async Task<IActionResult> SendSaleNotification(Guid saleId)
//{
//    // گرفتن اطلاعات فروش با استفاده از SaleId
//    var sale = await _context.Sales
//        .Where(s => s.Id == saleId)
//        .FirstOrDefaultAsync();

//    // اگر فروش با این شناسه پیدا نشد
//    if (sale == null)
//    {
//        return NotFound("فروش مورد نظر یافت نشد.");
//    }

//    // بررسی مشتری
//    var customer = await _context.buyers
//        .Where(c => c.Id == sale.Id)
//        .FirstOrDefaultAsync();

//    // اگر مشتری با این شناسه پیدا نشد
//    if (customer == null)
//    {
//        return NotFound("مشتری مورد نظر یافت نشد.");
//    }

//    // ایجاد نوتیفیکیشن جدید
//    //var notification = new Notification
//    //{
//    //    Title = "فروش جدید",
//    //    Message = $"یک خودرو به مبلغ {sale.Amount} با شناسه فروش {sale.Id} فروخته شد.",
//    //    CreatedAt = DateTime.Now,
//    //    IsRead = false,
//    //    CustomerId = customer.CustomerId
//    //};
//    var notification = new Notification
//    (
//        "فروش جدید",
//       $"یک خودرو به مبلغ {sale.Amount} با شناسه فروش {sale.Id} فروخته شد.",
//        DateTime.Now,
//        customer.Id
//    );

//    // ذخیره نوتیفیکیشن در دیتابیس
//    _context.Notifications.Add(notification);
//    await _context.SaveChangesAsync();

//    return Ok("نوتیفیکیشن با موفقیت ارسال شد.");
//}
