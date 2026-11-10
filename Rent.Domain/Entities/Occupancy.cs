using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rent.Domain.Entities
{
    public class Occupancy
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        [InverseProperty(nameof(User.Occupancies))]
        public User? User { get; set; }

        public int? FloorId { get; set; }
        [ForeignKey(nameof(FloorId))]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        [InverseProperty(nameof(Floor.Occupancies))]
        public Floor? Floor { get; set; }

        public int? FlatId { get; set; }
        [ForeignKey(nameof(FlatId))]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        [InverseProperty(nameof(Flat.Occupancies))]
        public Flat? Flat { get; set; }

        public int? RoomId { get; set; }
        [ForeignKey(nameof(RoomId))]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        [InverseProperty(nameof(Room.Occupancies))]
        public Room? Room { get; set; }

        public int? BedId { get; set; }
        [ForeignKey(nameof(BedId))]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        [InverseProperty(nameof(Bed.Occupancies))]
        public Bed? Bed { get; set; }

        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }


        // NVP
        [InverseProperty(nameof(Bill.Occupancy))]
        public ICollection<Bill> Bills { get; set; } = new List<Bill>();
    }
}
