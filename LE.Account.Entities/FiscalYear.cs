using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LE.Account.Entities
{
    public class FiscalYear
    {
        [Key] public long fiscal_year_id { get; set; }

        [Required] public string month { get; set; }

        [Required] public string day { get; set; }
    }

    public class FinancialYear
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool Running { get; set; }

        public bool Closed { get; set; }

        public string RecStatus { get; set; }

        public DateTime? ClosedDate { get; set; }

        public int Status { get; set; }

        public decimal OpeningStock { get; set; }
        public decimal ClosingStock { get; set; }
        public string Type { get; set; }
        public decimal PlAmount { get; set; }
    }
}