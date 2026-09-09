using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Billing.Entities
{
    public class WoodBillMembers
    {
        [Key]
        public long wood_bill_member_id { get; set; }

        [Required]
        public long wood_bill_id { get; set; }

        [ForeignKey("wood_bill_id")]
        public virtual WoodBill woodBill { get; set; }

        [Required]
        public long member_id { get; set; }

        [ForeignKey("member_id")]
        public virtual Member member { get; set; }
    }
}
