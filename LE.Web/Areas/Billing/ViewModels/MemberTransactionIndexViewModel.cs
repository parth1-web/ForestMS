using DateConverter.Core.Service_Factory;
using LE.Billing.Entities;
using LE.Inventory.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Web.Areas.Billing.ViewModels
{
    public class MemberTransactionIndexViewModel
    {
        [Display(Name = "From Date")]
        public string start_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        [Display(Name = "To Date")]
        public string end_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        public bool is_cancelled { get; set; } = false;

        public long member_id { get; set; }

        public List<MemberTransactionDetails> member_transactions = new List<MemberTransactionDetails>();
    }

    public class MemberTransactionDetails
    {
        public long wood_bill_member_transaction_id { get; set; }

        public long wood_bill_id { get; set; }

        public virtual WoodBill woodBill { get; set; }

        public long member_id { get; set; }

        public virtual Member member { get; set; }

        public long wood_details_id { get; set; }
        public virtual WoodDetails woodDetail { get; set; }

        public decimal rate { get; set; }

        public bool is_cancelled { get; set; }

        public decimal amount { get; set; }

        public decimal quantity { get; set; }
    }

}
