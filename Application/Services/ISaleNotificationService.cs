using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ISaleNotificationService
    {
        Task SendSaleNotificationAsync(Guid saleId);
        Task SendPaymentConfirmationNotificationAsync(Guid saleId);
    }
}
