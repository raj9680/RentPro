using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rent.Domain.Entities
{
    public class Floor
    {
        [Key]
        public int Id { get; set; }

        public int PropId { get; set; }
        [ForeignKey(nameof(PropId))]
        [InverseProperty(nameof(Property.Floors))]
        public Property? Property { get; set; } // NVP

        public string? Name { get; set; }


        [InverseProperty(nameof(Flat.Floor))]
        public ICollection<Flat> Flats { get; set; } = new List<Flat>(); // NVP


        [InverseProperty(nameof(FloorAmenity.Floor))]
        public ICollection<FloorAmenity> FloorAmenities { get; set; } = new List<FloorAmenity>(); // NVP
    }
}
