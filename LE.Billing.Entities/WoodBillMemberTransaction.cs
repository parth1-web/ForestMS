using LE.Inventory.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Billing.Entities
{
    public class WoodBillMemberTransaction
    {
        [Key]
        public long wood_bill_member_transaction_id { get; set; }

        [Required]
        public long wood_bill_id { get; set; }

        [ForeignKey("wood_bill_id")]
        public virtual WoodBill woodBill { get; set; }

        [Required]
        public long member_id { get; set; }

        [ForeignKey("member_id")]
        public virtual Member member { get; set; }

        [Required]
        public long wood_details_id { get; set; }

        [ForeignKey("wood_details_id")]
        public virtual WoodDetails woodDetail { get; set; }

        public decimal rate { get; set; }

        public decimal amount { get; set; }

        public decimal tax_amount { get; set; }

        public bool is_cancelled { get; set; } = false;

        [Required]
        public decimal quantity { get; set; }
    }
}
