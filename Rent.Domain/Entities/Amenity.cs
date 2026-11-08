using System.ComponentModel.DataAnnotations;

namespace Rent.Domain.Entities
{
    public class Amenity
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }

        public ICollection<FloorAmenity> FloorAmenities { get; set; } = new List<FloorAmenity>(); // NVP
    }
}
