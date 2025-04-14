using System.ComponentModel.DataAnnotations.Schema;
using Domain.Model;

namespace Carproject.Model
{
    public class FileCar:FileBase
    {
        public Guid CarId { get; set; }
        [ForeignKey("CarId")]
        public Car Car { get; set; }

        // سازنده‌ای که مقدارهای لازم را به FileBase می‌فرستد
        public FileCar(string fileName, string filePath, Guid carId) : base(fileName, filePath,  carId)
        {
            
        }
    }

}

