using System.ComponentModel.DataAnnotations;
using Domain.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Model.File
{
    public  class FileBase
    {
        public FileBase()
        {
        }

        public FileBase(string fileName, string filePath)
        {
            Id = Guid.NewGuid();
            FileName = fileName;
            FilePath = filePath;

        }

        [Key]
        public Guid Id { get; private set; }
        public string FileName { get; private set; }
        public string FilePath { get; private set; }
        

    }
}
