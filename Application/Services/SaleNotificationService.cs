using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Model;
using Domain.Model.ReportNotifModel;
using Domain.Model.UserModel;
using Infrustructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class SaleNotificationService : ISaleNotificationService
    {
        private readonly CARdbcontext _context;
        private readonly IEmailService _emailService;

        public SaleNotificationService(CARdbcontext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task SendSaleNotificationAsync(Guid saleId)
        {
            var sale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == saleId);
            if (sale == null) throw new Exception("فروش یافت نشد.");

            var buyer = await _context.buyers.FirstOrDefaultAsync(c => c.Id == sale.BuyerId);
            if (buyer == null) throw new Exception("مشتری یافت نشد.");

            // ایجاد نوتیفیکیشن
            var notification = new Notification(
                "فروش جدید",
                $"یک خودرو به مبلغ {sale.Amount} با شناسه فروش {sale.Id} فروخته شد.",
                DateTime.Now,
                sale.CarId,
                buyer.Id
            );
           
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            
               
            // ارسال ایمیل
            var emailRequest = new Email
            {
                To = buyer.Email, // فرض: Email داخل مدل customer هست
                Subject = "تأیید خرید",
                Body = $"<html><body>با تشکر از خرید شما. مبلغ خرید: {sale.Amount}</body></html>"
            };

            await _emailService.SendEmailAsync(emailRequest);
        }

        public async Task SendSabtsefarchNotificationAsync(Guid saleId)
        {
            var sale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == saleId);
            if (sale == null) throw new Exception("فروش یافت نشد.");

            var buyer = await _context.buyers.FirstOrDefaultAsync(c => c.Id == sale.BuyerId);
            if (buyer == null) throw new Exception("مشتری یافت نشد.");

            // ایجاد نوتیفیکیشن
            var notification = new Notification(
                "فروش جدید",
                $"یک خودرو به مبلغ {sale.Amount} با شناسه فروش {sale.Id} به سبد خرید شما اضافه شد.",
                DateTime.Now,
                sale.CarId,
                buyer.Id
            );

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();


            // ارسال ایمیل
            var emailRequest = new Email
            {
                To = buyer.Email, // فرض: Email داخل مدل customer هست
                Subject = "تأیید خرید",
                Body = $"<html><body>با تشکر از سفارش شما. مبلغ خرید: {sale.Amount}</body></html>"
            };

            await _emailService.SendEmailAsync(emailRequest);
        }
        public async Task SendPaymentConfirmationNotificationAsync(Guid saleId)
        {
            var sale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == saleId);
            if (sale == null) throw new Exception("فروش یافت نشد.");

            var buyer = await _context.buyers.FirstOrDefaultAsync(c => c.Id == sale.BuyerId);
            if (buyer == null) throw new Exception("مشتری یافت نشد.");

            // ایجاد نوتیفیکیشن تأیید پرداخت
            var notification = new Notification(
                "تأیید پرداخت",
                $"پرداخت شما به مبلغ {sale.Amount} برای خرید با شناسه {sale.Id} با موفقیت انجام شد.",
                DateTime.Now,
                sale.CarId,
                buyer.Id
            );

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // ارسال ایمیل تأیید پرداخت
            var emailRequest = new Email
            {
                To = buyer.Email,
                Subject = "تأیید پرداخت",
                Body = $"<html><body><p>پرداخت شما به مبلغ <strong>{sale.Amount}</strong> با موفقیت انجام شد.</p><p>از خرید شما سپاسگزاریم 🙏</p></body></html>"
            };

            await _emailService.SendEmailAsync(emailRequest);
        }
    }
}
