using System.ComponentModel.DataAnnotations;
using Domain.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carproject.Model
{
    public class FileBase
    {
        public FileBase()
        {
        }

        public FileBase(string fileName, string filePath, Guid carId)
        {
            FileName = fileName;
            FilePath = filePath;
            CarId = carId;

        }

        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Guid CarId { get; set; }

        //[ForeignKey("CarId")]
        //public Car Car { get; set; }
    }
}
