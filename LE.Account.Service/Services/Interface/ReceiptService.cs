using LE.Account.Infrastructure.Dto;

namespace LE.Account.Service.Services.Interface
{
    public interface ReceiptService
    {
        long makeReceipt(ReceiptDto receiptDto);
        void cancel(long receipt_id, long user_id);
    }
}
