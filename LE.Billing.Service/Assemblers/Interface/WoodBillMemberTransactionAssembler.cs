using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface WoodBillMemberTransactionAssembler
    {
        void copy(WoodBillMemberTransaction woodBillMemberTransaction, WoodBillMemberTransactionDto woodBillMemberTransactionDto);
    }
}
