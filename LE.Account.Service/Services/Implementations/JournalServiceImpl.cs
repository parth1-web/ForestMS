using LE.Account.Common.Enums;
using LE.Account.Infrastructure.Dto;
using LE.Account.Service.Assemblers.Implementations;
using LE.Account.Service.Services.Interface;

namespace LE.Account.Service.Services.Implementations
{
    public class JournalServiceImpl : JournalService
    {
        private TransactionDtoAssembler _transactionDtoMaker;
        private TransactionService _transactionService;

        public JournalServiceImpl(TransactionService transactionService, TransactionDtoAssembler transactionDtoMaker)
        {

            _transactionDtoMaker = transactionDtoMaker;
            _transactionService = transactionService;
        }

        public void makeJournalEntries(JournalDto journalDto)
        {
            var transactionDto = new TransactionDto();

            foreach (var dto in journalDto.journalDetailDto)
            {
                if (dto.dr_amount > 0)
                {
                    transactionDto.addDebitData(new LedgerTransactionDto()
                    {
                        amount = dto.dr_amount,
                        ledger_id = dto.ledger_id
                    });
                }

                if (dto.cr_amount > 0)
                {
                    transactionDto.addCreditData(new LedgerTransactionDto()
                    {
                        amount = dto.cr_amount,
                        ledger_id = dto.ledger_id,
                    });
                }
            }
            transactionDto.remarks = journalDto.remarks;
            transactionDto.voucher_no = journalDto.voucher_no;
            transactionDto.voucher_type = VoucherType.Journal;
            transactionDto.transaction_date = journalDto.transaction_date;
            _transactionService.addTransaction(transactionDto);
        }
    }
}
