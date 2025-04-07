using System.ComponentModel.DataAnnotations;

namespace Carproject.Model
{
    public class FileBase
    {
        //public FileBase()
        //{
        //}

        public FileBase(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
        }

        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
