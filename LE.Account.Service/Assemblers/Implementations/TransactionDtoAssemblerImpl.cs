using DateConverter.Core.Service_Factory;
using LE.Account.Common.Enums;
using LE.Account.Infrastructure.Dto;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Common.Exceptions;
using System;

namespace LE.Account.Service.Assemblers.Implementations
{
    public class TransactionDtoAssemblerImpl : TransactionDtoAssembler
    {
        private LedgerSetupRepository ledgerSetupRepo;

        public TransactionDtoAssemblerImpl(LedgerSetupRepository _ledgerSetupRepo)
        {
            ledgerSetupRepo = _ledgerSetupRepo;
        }

        public TransactionDto createTransactionDtoFrom(LedgerDto ledgerDto)
        {
            TransactionDto transactionDto = new TransactionDto();
            LedgerTransactionDto debitTransactionDetailDto = new LedgerTransactionDto();
            LedgerTransactionDto creditTransactionDetailDto = new LedgerTransactionDto();

            switch (ledgerDto.balance_type)
            {
                case OpeningBalanceType.debit:
                    debitTransactionDetailDto.ledger_id = ledgerDto.ledger_id;
                    debitTransactionDetailDto.amount = ledgerDto.opening_balance;

                    creditTransactionDetailDto.ledger_id = 0;
                    creditTransactionDetailDto.amount = ledgerDto.opening_balance;
                    break;

                case OpeningBalanceType.credit:
                    creditTransactionDetailDto.ledger_id = ledgerDto.ledger_id;
                    creditTransactionDetailDto.amount = ledgerDto.opening_balance;

                    debitTransactionDetailDto.ledger_id = 0;
                    debitTransactionDetailDto.amount = ledgerDto.opening_balance;

                    break;
                default:
                    throw new InvalidValueException("Opening Balance Type is not specified.");

            }
            transactionDto.addCreditData(creditTransactionDetailDto);
            transactionDto.addDebitData(debitTransactionDetailDto);
            transactionDto.remarks = "Being ledger created for " + ledgerDto.name;
            transactionDto.transaction_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
            return transactionDto;

        }

        public TransactionDto createTransactionDtoFrom(PaymentDto paymentDto)
        {
            TransactionDto transactionDto = new TransactionDto();
            transactionDto.transaction_date = paymentDto.transaction_date;
            transactionDto.remarks = paymentDto.remarks;
            transactionDto.voucher_no = paymentDto.payment_id;
            transactionDto.voucher_type = VoucherType.Payment;

            LedgerTransactionDto debitTransactionDetailDto = new LedgerTransactionDto();
            LedgerTransactionDto creditTransactionDetailDto = new LedgerTransactionDto();

            debitTransactionDetailDto.ledger_id = paymentDto.payment_to;
            debitTransactionDetailDto.amount = paymentDto.amount + paymentDto.discount;
            transactionDto.addDebitData(debitTransactionDetailDto);

            creditTransactionDetailDto.ledger_id = paymentDto.payment_from;
            creditTransactionDetailDto.amount = paymentDto.amount;
            transactionDto.addCreditData(creditTransactionDetailDto);

            if (paymentDto.discount > 0)
            {
                LedgerTransactionDto crTransactionDetailDto = new LedgerTransactionDto();
                //check whether settings is available or not
                Entities.LedgerSetup discount_setting = ledgerSetupRepo.getByKey(LedgerSetup.discount_received.ToString());
                if (discount_setting == null)
                    throw new ItemNotFoundException("No setup found for discount received.");
                crTransactionDetailDto.ledger_id = Convert.ToInt32(discount_setting.value);
                crTransactionDetailDto.amount = paymentDto.discount;
                transactionDto.addCreditData(crTransactionDetailDto);
            }

            return transactionDto;
        }

        public TransactionDto createTransactionDtoFrom(ReceiptDto receiptDto)
        {
            TransactionDto transactionDto = new TransactionDto();
            transactionDto.transaction_date = receiptDto.transaction_date;
            transactionDto.remarks = receiptDto.remarks;
            transactionDto.voucher_no = receiptDto.receipt_id;
            transactionDto.voucher_type = VoucherType.Receipt;
            LedgerTransactionDto debitTransactionDetailDto = new LedgerTransactionDto();
            LedgerTransactionDto creditTransactionDetailDto = new LedgerTransactionDto();

            debitTransactionDetailDto.ledger_id = receiptDto.receipt_to;
            debitTransactionDetailDto.amount = receiptDto.amount + receiptDto.discount;
            transactionDto.addDebitData(debitTransactionDetailDto);

            creditTransactionDetailDto.ledger_id = receiptDto.receipt_from;
            creditTransactionDetailDto.amount = receiptDto.amount;
            transactionDto.addCreditData(creditTransactionDetailDto);

            if (receiptDto.discount > 0)
            {
                // P1/B11 fix: this branch mutated 'creditTransactionDetailDto' (already added
                // above), overwriting the receipt's credit line with the discount ledger and
                // breaking Dr = Cr for every receipt with a discount. The discount line is
                // now its own DTO instance.
                LedgerTransactionDto crTransactionDetailDto = new LedgerTransactionDto();
                //check whether settings is available or not
                Entities.LedgerSetup discount_setting = ledgerSetupRepo.getByKey(LedgerSetup.discount_allowed.ToString());
                if (discount_setting == null)
                    throw new ItemNotFoundException("No setup found for discount allowed.");
                crTransactionDetailDto.ledger_id = Convert.ToInt32(discount_setting.value);
                crTransactionDetailDto.amount = receiptDto.discount;
                transactionDto.addCreditData(crTransactionDetailDto);
            }
            return transactionDto;
        }

        //public TransactionDto createTransactionDtoFrom(JournalDto journalDto)
        //{
        //    TransactionDto transactionDto = new TransactionDto();
        //    transactionDto.transaction_date = journalDto.transaction_date;
        //    transactionDto.remarks = journalDto.remarks;
        //    LedgerTransactionDto debitTransactionDetailDto = new LedgerTransactionDto();
        //    LedgerTransactionDto creditTransactionDetailDto = new LedgerTransactionDto();

        //    debitTransactionDetailDto.ledger_id = journalDto.to_ledger_id;
        //    debitTransactionDetailDto.amount = journalDto.dr_amount;
        //    transactionDto.addDebitData(debitTransactionDetailDto);

        //    creditTransactionDetailDto.ledger_id = journalDto.from_ledger_id;
        //    creditTransactionDetailDto.amount = journalDto.dr_amount;
        //    transactionDto.addCreditData(creditTransactionDetailDto);
        //    return transactionDto;
        //}
    }
}
