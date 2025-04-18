using System.ComponentModel.DataAnnotations;

namespace Domain.Model.CarModel
{
    public class CarCategory
    {
        public CarCategory( string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        [Key]
        public Guid Id { get; private set; }

        [Required]
        public string Name { get;private set; }


    }
}
