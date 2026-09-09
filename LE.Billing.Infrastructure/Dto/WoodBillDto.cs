using LE.Billing.Common.Enums;
using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Infrastructure.Dto
{
    public class WoodBillDto
    {
        decimal _amount;

        public long wood_bill_id { get; set; }

        public DateTime bill_date { get; set; }

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

        public string address { get; set; }

        public bool is_cancelled { get; set; } 



        private List<WoodBillDetailDto> woodBillDetailDto = new List<WoodBillDetailDto>();

        public List<WoodBillDetailDto> wood_detail_dto  => woodBillDetailDto;

        public void addWoodBillDetails(List<WoodBillDetailDto> wood_bill_detail_dto)
        {
            foreach(var dto in wood_bill_detail_dto)
            {
                woodBillDetailDto.Add(dto);

            }
        }

        private List<WoodBillMemberDto> woodBillMemberDto = new List<WoodBillMemberDto>();
        
        public List<WoodBillMemberDto> wood_bill_member_dto => woodBillMemberDto;

        public void addWoodMemberDetails(List<WoodBillMemberDto> wood_bill_member_dtos)
        {
            foreach(var dto in wood_bill_member_dtos)
            {
                woodBillMemberDto.Add(dto);
            }
            
        }

        private List<WoodBillMemberTransactionDto> woodBillTransactionDto = new List<WoodBillMemberTransactionDto>();

        public List<WoodBillMemberTransactionDto> wood_bill_member_transaction_dto => woodBillTransactionDto;

        public void addWoodMemberTransactionDetails(List<WoodBillMemberTransactionDto> wood_bill_member_transaction_dtos)
        {
            foreach (var dto in wood_bill_member_transaction_dtos)
            {
                woodBillTransactionDto.Add(dto);
            }

        }

    }
}
