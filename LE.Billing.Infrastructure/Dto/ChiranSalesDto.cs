using LE.Billing.Common.Enums;
using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Infrastructure.Dto
{
    public class ChiranSalesDto
    {
        decimal _amount;

        public long chiran_sales_id { get; set; }

        public DateTime sales_date { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal amount
        {
            get => _amount;
            set
            {
                if (value < 0)
                {
                    throw new InvalidValueException("Amount cannot be Negative.");
                }
                _amount = value;
            }
        }

        [MaxLength(150)]
        public string remarks { get; set; }

        public long user_id { get; set; }

        public decimal tax_amount { get; set; }

        public string name { get; set; }

        public SalesType sales_type { get; set; }

        public long? member_id { get; set; }

        public string address { get; set; }

        public bool is_cancelled { get; set; } 


        private List<ChiranSalesDetailDto> chiranSalesDetailDto = new List<ChiranSalesDetailDto>();

        public List<ChiranSalesDetailDto> chiran_detail_dto => chiranSalesDetailDto;

        public void addChiranBillDetails(List<ChiranSalesDetailDto> chiran_sales_detail_dto)
        {
            foreach(var dto in chiran_sales_detail_dto)
            {
                chiranSalesDetailDto.Add(dto);

            }
        }

    }
}
