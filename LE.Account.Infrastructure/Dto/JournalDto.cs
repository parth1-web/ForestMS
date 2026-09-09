using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LE.Account.Infrastructure.Dto
{
    public class JournalDto
    {
        [Required]
        public DateTime transaction_date { get; set; }

        public string remarks { get; set; }

        public long voucher_no { get; set; }

        public List<JournalDetailDto> journalDetailDto { get; set; }

    }

    public class JournalDetailDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ledger is required.")]
        public long ledger_id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debit Amount is required. ")]
        public decimal dr_amount { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Credit Amount is required. ")]
        public decimal cr_amount { get; set; }
        
    }
}
