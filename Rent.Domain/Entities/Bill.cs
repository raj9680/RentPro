using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rent.Domain.Entities
{
    public class Bill
    {
        [Key]
        public int Id { get; set; }
        public int OccupancyId { get; set; }

        [ForeignKey(nameof(OccupancyId))]
        [InverseProperty(nameof(Occupancy.Bills))]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public Occupancy? Occupancy { get; set; }

        public DateOnly BillingMonth { get; set; }
        
        [Precision(18, 2)]
        public decimal Amount { get; set; }
        public DateOnly DueDate { get; set; }


        // NVP
        [InverseProperty(nameof(Payment.Bill))]
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
