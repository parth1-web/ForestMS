using DateConverter.Core.Service_Factory;
using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Assemblers.Interface;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Account.Service.Assemblers.Implementations
{
    public class TransactionAssemblerImpl : TransactionAssembler
    {
        private AccountSettingsRepository _transactionSequenceRepo;
        public TransactionAssemblerImpl(AccountSettingsRepository transactionSequenceRepo)
        {
            _transactionSequenceRepo = transactionSequenceRepo;
        }
        public void copy(Transaction transaction, TransactionDto transaction_dto)
        {
            transaction.transaction_id = _transactionSequenceRepo.getTransactionSequence();
            transaction.entry_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
            transaction.transaction_date = transaction_dto.transaction_date;
            transaction.remarks = transaction_dto.remarks;
            transaction.voucher_no = transaction_dto.voucher_no;
            transaction.voucher_type = transaction_dto.voucher_type;
            var dateConverterService = DateConverterFactory.getDateConverterService();
            transaction.nep_entry_date = dateConverterService.ToBS(transaction.entry_date.Date, DateFormats.yMd).getFormattedDate().ToString();
            transaction.nep_transaction_date = dateConverterService.ToBS(transaction.transaction_date.Date, DateFormats.yMd).getFormattedDate().ToString();
        }
    }
}
