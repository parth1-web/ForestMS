using LE.Account.Infrastructure.Dto;

namespace LE.Account.Service.Assemblers.Implementations
{
    public interface TransactionDtoAssembler
    {
        TransactionDto createTransactionDtoFrom(LedgerDto ledgerDto);
        TransactionDto createTransactionDtoFrom(PaymentDto paymentDto);
        TransactionDto createTransactionDtoFrom(ReceiptDto receiptDto);
        // TransactionDto createTransactionDtoFrom(JournalDto journalDto);
    }
}
