using LE.Billing.Common.Enums;
using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Infrastructure.Dto
{
    public class WoodBillMemberTransactionDto
    {
        public long wood_bill_member_transaction_id { get; set; }

        public long wood_bill_id { get; set; }

        public long member_id { get; set; }
        public long wood_details_id { get; set; }

        public decimal rate { get; set; }

        public decimal amount { get; set; }
        public decimal tax_amount { get; set; }

        public decimal quantity { get; set; }
    }
}
