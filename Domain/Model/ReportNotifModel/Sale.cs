using Domain.Model.UserModel;
using Domain.Model.CarModel;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Model.ReportNotifModel
{
    public class Sale
    {



        public Guid Id { get; private set; }               // شناسه فروش
        public DateTime SaleDate { get; private set; }    // تاریخ فروش
        public decimal Amount { get; private set; }      // مبلغ فروش
        public Guid BuyerId { get; private set; }       // شناسه مشتری
        public Guid CarId { get; private set; }        // شناسه خودرو
        public Buyer Buyer { get; private set; }      // اطلاعات مشتری
        public Car Car { get; private set; }         // اطلاعات خودرو
        public int? stock { get; private set; }     // تعدادماشین های موجود
        public bool Ispay { get; private set; } = false; // تایید خرید

        // فیلد برای ذخیره فایل‌ها در دیتابیس (رشته‌ی جوین‌شده)
        public string? FilesIds { get; private set; }

        // پراپرتی کمکی برای کار با فایل‌ها به صورت لیست
        public List<string> FileIdsList
        {
            get => string.IsNullOrWhiteSpace(FilesIds)
                ? new List<string>()
                : FilesIds.Split(',').ToList();

            private set => FilesIds = string.Join(",", value);
        }


        public Sale()
        {

        }
        public void MarkAsPaid()
        {
            Ispay = true;
        }

        public void AddFileId(Guid fileId)
        {
            var list = string.IsNullOrWhiteSpace(FilesIds)
                ? new List<string>()
                : FilesIds.Split(',').ToList();

            list.Add(fileId.ToString());

            FilesIds = string.Join(",", list);
        }

        public void SetFilesIds(List<string> fileIdsList)
        {
            FilesIds = string.Join(",", fileIdsList);
        }

        public List<string> GetFileIdsList()
        {
            return string.IsNullOrWhiteSpace(FilesIds)
                ? new List<string>()
                : FilesIds.Split(',').ToList();
        }
        public void RemoveFileId(Guid fileId)
        {
            var list = GetFileIdsList();
            list.Remove(fileId.ToString());
            SetFilesIds(list);
        }




        public Sale(DateTime saleDate, decimal amount, Guid buyerId, Guid carId, bool ispay)
        {
            Id = Guid.NewGuid();
            SaleDate = saleDate;
            Amount = amount;
            BuyerId = buyerId;
            CarId = carId;
            Ispay = ispay;
            
        }
        public Sale(DateTime saleDate, decimal amount, Guid buyerId, Guid carId, bool ispay, string filesIds)
        {
            Id = Guid.NewGuid();
            SaleDate = saleDate;
            Amount = amount;
            BuyerId = buyerId;
            CarId = carId;
            Ispay = ispay;
            FilesIds = "";
        }

    }

}
