using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LE.Billing.Infrastructure.Dto
{
    public class CounterSalesDto
    {
        public List<CounterSalesDetailDto> counter_sales_datas { get; set; } = new List<CounterSalesDetailDto>();

        public long sales_id { get; set; }

        public long user_id { get; set; }

        [Required(ErrorMessage = "Bill amount is required.")]
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal bill_amount { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal discount_amount { get; set; }

        public DateTime sales_date { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Required]
        public decimal net_total { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal return_amount { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal tax { get; set; }

        [MaxLength(120)]
        public string remarks { get; set; }
        public long cancelled_by { get; set; }

        public List<CounterSalesDetailDto> counter_sales_details => counter_sales_datas;

        public void addSalesDatas(CounterSalesDetailDto counter_sales_detail_dto)
        {
            counter_sales_datas.Add(counter_sales_detail_dto);
        }

        public bool isNetTotalValid()
        {
            return net_total == bill_amount - discount_amount;
        }

        public bool isDiscountAmountValid()
        {
            return discount_amount <= bill_amount;
        }
    }
}
