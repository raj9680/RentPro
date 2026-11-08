using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rent.Domain.Entities
{
    public class Flat
    {
        [Key]
        public int Id { get; set; }

        public int FloorId { get; set; }
        [ForeignKey(nameof(FloorId))]
        [InverseProperty(nameof(Floor.Flats))]
        public Floor? Floor { get; set; }

        public string? Name { get; set; }


        [InverseProperty(nameof(Room.Flat))]
        public ICollection<Room> Rooms { get; set; } = new List<Room>(); // NVP
    }
}
