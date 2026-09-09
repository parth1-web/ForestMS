using LE.Billing.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Infrastructure.Dto
{
    public class FurnitureSalesDto
    {
        private List<FurnitureSalesDetailDto> furniture_sales_datas = new List<FurnitureSalesDetailDto>();

        public long furniture_sales_id { get; set; }

        public DateTime sales_date { get; set; }

        public FirewoodSalesType sales_type { get; set; }

        public long? type_id { get; set; }

        public string others_name { get; set; }

        public string address { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal total_amount { get; set; }

        [MaxLength(150)]
        public string remarks { get; set; }

        public long user_id { get; set; }

        public bool is_cancelled { get; set; } = false;

        public long cancelled_by { get; set; }

        public List<FurnitureSalesDetailDto> furniture_sales_details => furniture_sales_datas;

        public void addSalesDatas(FurnitureSalesDetailDto furniture_sales_detail_dto)
        {
            furniture_sales_datas.Add(furniture_sales_detail_dto);
        }
    }
}
