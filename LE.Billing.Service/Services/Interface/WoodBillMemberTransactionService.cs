using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Services.Interface
{
    public interface WoodBillMemberTransactionService
    {
        void insert(List<WoodBillMemberTransactionDto> wood_bill_member_transaction_dto);
    }
}
