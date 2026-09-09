namespace LE.Account.Infrastructure.Reports.Dto
{
    public class AccountingReportQueryValueDto
    {
        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int LedgerId { get; set; }
        public string LedgerName { get; set; }
        public string LedgerCode { get; set; }
        public decimal Balance { get; set; }
        public decimal DrAmount { get; set; }
        public decimal CrAmount { get; set; }
        public decimal PreviousDrAmount { get; set; }
        public decimal PreviousCrAmount { get; set; }
        public decimal PreviousBalance { get; set; }
        public string GroupCode { get; set; }
        public string LedgerType { get; set; }
    }
}