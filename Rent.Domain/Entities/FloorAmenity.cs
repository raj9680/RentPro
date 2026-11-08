using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rent.Domain.Entities
{
    public class FloorAmenity
    {
        [Key]
        public int Id { get; set; }
        public int FloorId { get; set; }

        [ForeignKey(nameof(FloorId))]
        [InverseProperty(nameof(Floor.FloorAmenities))]
        public Floor? Floor { get; set; } // NVP

        public int AmenityId { get; set; }
        [ForeignKey(nameof(AmenityId))]
        [InverseProperty(nameof(Amenity.FloorAmenities))]

        public Amenity? Amenity { get; set; } // NVP
    }
}
