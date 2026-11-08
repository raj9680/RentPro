using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rent.Domain.Entities
{
    public class Bed
    {
        [Key]
        public int Id { get; set; }

        public int RoomId { get; set; }
        [ForeignKey(nameof(RoomId))]
        [InverseProperty(nameof(Room.Beds))]
        public Room? Room { get; set; }

        public string? Name { get; set; }
    }
}
