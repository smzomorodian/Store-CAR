using System.ComponentModel.DataAnnotations;

namespace Application.DTO.CarDTO
{
    public class CarCategoryDto
    {
        [Required]
        public string Name { get; set; }

    }
}
