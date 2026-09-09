using LE.Billing.Common.Enums;
using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Infrastructure.Dto
{
    public class WoodBillMemberDto
    {
        public long wood_bill_member_id { get; set; }

        public long wood_bill_id { get; set; }

        public long member_id { get; set; }
    }
}
