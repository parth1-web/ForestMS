using System.Collections.Generic;

namespace LE.Account.Infrastructure.Reports.Reporter.ValueObjects
{
    public class AccountingReportVo
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal Dr { get; set; }
        public decimal Cr { get; set; }
        public decimal Balance { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal Total { get; set; }
        public bool HasChildren => Children.Count > 0;

        public List<AccountingReportVo> Children { get; set; } = new List<AccountingReportVo>();

        public AccountingReportVo Push(AccountingReportVo item)
        {
            Children.Add(item);
            return this;
        }
    }

}