using System.ComponentModel.DataAnnotations;

namespace Domain.Model
{
    public class CarCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }


    }
}
