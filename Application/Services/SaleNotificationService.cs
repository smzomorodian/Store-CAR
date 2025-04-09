using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carproject.Model;
using Domain.Model;
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

            var customer = await _context.buyers.FirstOrDefaultAsync(c => c.Id == sale.Id);
            if (customer == null) throw new Exception("مشتری یافت نشد.");

            // ایجاد نوتیفیکیشن
            var notification = new Notification(
                "فروش جدید",
                $"یک خودرو به مبلغ {sale.Amount} با شناسه فروش {sale.Id} فروخته شد.",
                DateTime.Now,
                customer.Id
            );

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // ارسال ایمیل
            var emailRequest = new Email
            {
                To = customer.Email, // فرض: Email داخل مدل customer هست
                Subject = "تأیید خرید",
                Body = $"<html><body>با تشکر از خرید شما. مبلغ خرید: {sale.Amount}</body></html>"
            };

            await _emailService.SendEmailAsync(emailRequest);
        }
    }

}
