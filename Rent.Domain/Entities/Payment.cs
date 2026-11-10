using Microsoft.EntityFrameworkCore;
using Rent.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rent.Domain.Entities
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public int BillId { get; set; }

        [ForeignKey(nameof(BillId))]
        [InverseProperty(nameof(Bill.Payments))]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public Bill? Bill { get; set; }

        [Precision(18, 2)]
        public decimal Amount { get; set; }
        public DateOnly PaidOn { get; set; }
        public Mode Mode { get; set; }
    }
}
