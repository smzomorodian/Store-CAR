using System.ComponentModel.DataAnnotations;

namespace Application.DTO
{
    public class CarCategoryDto
    {
        [Required]
        public string Name { get; set; }

    }
}
