using DateConverter.Core.Service_Factory;
using LE.Account.Common.Enums;
using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Assemblers.Implementations;
using LE.Account.Service.Assemblers.Interface;
using LE.Account.Service.Services.Interface;
using LE.Common.Exceptions;
using System;
using System.Transactions;

namespace LE.Account.Service.Services.Implementations
{
    public class ReceiptServiceImpl : ReceiptService
    {
        ReceiptRepository _receiptRepo;
        TransactionService _transactionService;
        ReceiptAssembler _receiptMaker;
        TransactionDtoAssembler _transactionDtoMaker;
        private readonly LedgerSetupRepository _ledgerSetupRepo;

        public ReceiptServiceImpl(TransactionDtoAssembler transactionDtoMaker, ReceiptAssembler receiptMaker, ReceiptRepository receiptRepo, TransactionService transactionService, LedgerSetupRepository ledgerSetupRepo)
        {
            _receiptRepo = receiptRepo;
            _transactionService = transactionService;
            _receiptMaker = receiptMaker;
            _transactionDtoMaker = transactionDtoMaker;
            _ledgerSetupRepo = ledgerSetupRepo;
        }

        public void cancel(long receipt_id, long user_id)
        {
            using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
            {
                var receipt = _receiptRepo.getById(receipt_id);
                if (receipt == null)
                {
                    throw new ItemNotFoundException("Receipt doesnot exist.");
                }
                if (receipt.is_cancelled == true)
                {
                    throw new ItemUsedException("Receipt is already cancelled.");
                }

                TransactionDto transactionDto = getTransactionDtoForReverseEntry(receipt);
                _transactionService.addTransaction(transactionDto);

                receipt.is_cancelled = true;
                receipt.cancelled_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
                receipt.cancelled_by = user_id;
                _receiptRepo.update(receipt);
                tx.Complete();
            }

        }

        private TransactionDto getTransactionDtoForReverseEntry(Receipt receipt)
        {
            TransactionDto transactionDto = new TransactionDto();
            transactionDto.transaction_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
            transactionDto.voucher_no = receipt.receipt_id;
            transactionDto.voucher_type = VoucherType.Receipt;
            transactionDto.remarks = "Being Receipt Cancelled";
            LedgerTransactionDto debitTransactionDetailDto = new LedgerTransactionDto();
            LedgerTransactionDto creditTransactionDetailDto = new LedgerTransactionDto();

            creditTransactionDetailDto.ledger_id = receipt.receipt_to;
            creditTransactionDetailDto.amount = receipt.amount + receipt.discount;
            transactionDto.addCreditData(creditTransactionDetailDto);

            debitTransactionDetailDto.ledger_id = receipt.receipt_from;
            debitTransactionDetailDto.amount = receipt.amount;
            transactionDto.addDebitData(debitTransactionDetailDto);

            if (receipt.discount > 0)
            {
                TransactionDetailDto crTransactionDetailDto = new TransactionDetailDto();
                //check whether settings is available or not
                Entities.LedgerSetup discount_setting = _ledgerSetupRepo.getByKey(LE.Account.Common.Enums.LedgerSetup.discount_allowed.ToString());
                if (discount_setting == null)
                    throw new ItemNotFoundException("No setup found for discount allowed.");
                debitTransactionDetailDto.ledger_id = Convert.ToInt32(discount_setting.value);
                debitTransactionDetailDto.amount = receipt.discount;
                transactionDto.addDebitData(debitTransactionDetailDto);
            }

            return transactionDto;
        }

        public long makeReceipt(ReceiptDto receiptDto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    if (!receiptDto.isValid())
                        throw new InvalidValueException("The provided data are not valid.");
                    Receipt receipt = new Receipt();
                    _receiptMaker.copy(receipt, receiptDto);
                    _receiptRepo.insert(receipt);
                    receiptDto.receipt_id = receipt.receipt_id;
                    TransactionDto transactionDto = _transactionDtoMaker.createTransactionDtoFrom(receiptDto);

                    _transactionService.addTransaction(transactionDto);
                    tx.Complete();
                    return receipt.receipt_id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
