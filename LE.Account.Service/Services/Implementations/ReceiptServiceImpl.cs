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
            // P1 fix: ambient TransactionScope was a no-op for EF Core; the reverse
            // ledger entry + receipt update now run in one real database transaction.
            using (var tx = _receiptRepo.beginTransaction())
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
                _receiptRepo.saveChanges();
                tx.Commit();
            }

        }

        private TransactionDto getTransactionDtoForReverseEntry(Receipt receipt)
        {
            TransactionDto transactionDto = new TransactionDto();
            transactionDto.transaction_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
            transactionDto.voucher_no = receipt.receipt_id;
            transactionDto.voucher_type = VoucherType.Receipt;
            transactionDto.remarks = "Being Receipt Cancelled";

            // P1/B11 fix: this method mutated 'debitTransactionDetailDto' after adding it
            // to the transaction (the discount branch overwrote its ledger/amount), so
            // the original debit line vanished and Dr ≠ Cr. Each line is now its own
            // separate DTO instance.
            LedgerTransactionDto creditTransactionDetailDto = new LedgerTransactionDto();
            creditTransactionDetailDto.ledger_id = receipt.receipt_to;
            creditTransactionDetailDto.amount = receipt.amount + receipt.discount;
            transactionDto.addCreditData(creditTransactionDetailDto);

            LedgerTransactionDto debitTransactionDetailDto = new LedgerTransactionDto();
            debitTransactionDetailDto.ledger_id = receipt.receipt_from;
            debitTransactionDetailDto.amount = receipt.amount;
            transactionDto.addDebitData(debitTransactionDetailDto);

            if (receipt.discount > 0)
            {
                LedgerTransactionDto discountDebitDetailDto = new LedgerTransactionDto();
                //check whether settings is available or not
                Entities.LedgerSetup discount_setting = _ledgerSetupRepo.getByKey(LE.Account.Common.Enums.LedgerSetup.discount_allowed.ToString());
                if (discount_setting == null)
                    throw new ItemNotFoundException("No setup found for discount allowed.");
                discountDebitDetailDto.ledger_id = Convert.ToInt32(discount_setting.value);
                discountDebitDetailDto.amount = receipt.discount;
                transactionDto.addDebitData(discountDebitDetailDto);
            }

            return transactionDto;
        }

        public long makeReceipt(ReceiptDto receiptDto)
        {
            using (var tx = _receiptRepo.beginTransaction())
            {
                if (!receiptDto.isValid())
                    throw new InvalidValueException("The provided data are not valid.");
                Receipt receipt = new Receipt();
                _receiptMaker.copy(receipt, receiptDto);
                _receiptRepo.insert(receipt);
                receiptDto.receipt_id = receipt.receipt_id;
                TransactionDto transactionDto = _transactionDtoMaker.createTransactionDtoFrom(receiptDto);

                _transactionService.addTransaction(transactionDto);
                _receiptRepo.saveChanges();
                tx.Commit();
                return receipt.receipt_id;
            }
        }

    }
}
