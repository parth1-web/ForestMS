using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Service.Assemblers.Interface;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class WoodBillMemberTransactionAssemblerImpl : WoodBillMemberTransactionAssembler
    {
        public void copy(WoodBillMemberTransaction woodBillMemberTransaction, WoodBillMemberTransactionDto woodBillMemberTransactionDto)
        {
            woodBillMemberTransaction.wood_bill_id = woodBillMemberTransactionDto.wood_bill_id;
            woodBillMemberTransaction.member_id = woodBillMemberTransactionDto.member_id;
            woodBillMemberTransaction.rate = woodBillMemberTransactionDto.rate;
            woodBillMemberTransaction.amount = woodBillMemberTransactionDto.amount;
            woodBillMemberTransaction.tax_amount = woodBillMemberTransactionDto.tax_amount;
            woodBillMemberTransaction.wood_details_id = woodBillMemberTransactionDto.wood_details_id;
            woodBillMemberTransaction.quantity = woodBillMemberTransactionDto.quantity;
            
        }
    }
}
