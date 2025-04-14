using Domain.Model;

namespace Carproject.Model
{
    public class Sale
    {



        public Guid Id { get; set; }               // شناسه فروش
        public DateTime SaleDate { get; set; }    // تاریخ فروش
        public decimal Amount { get; set; }      // مبلغ فروش
        public Guid BuyerId { get; set; }       // شناسه مشتری
        public Guid CarId { get; set; }        // شناسه خودرو
        public Buyer Buyer { get; set; }      // اطلاعات مشتری
        public Car Car { get; set; }         // اطلاعات خودرو
        public int? stock { get; set; }     // تعدادماشین های موجود
        public bool Ispay { get; set; } = false; // تایید خرید
        public Sale()
        {

        }

        public Sale(DateTime saleDate, decimal amount, Guid buyerId, Guid carId , bool ispay)
        {
            Id = Guid.NewGuid();
            SaleDate = saleDate;
            Amount = amount;
            BuyerId = buyerId;
            CarId = carId;
            Ispay = ispay;
        }
    }

}
