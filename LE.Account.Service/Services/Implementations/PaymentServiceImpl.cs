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
    public class PaymentServiceImpl : PaymentService
    {
        PaymentAssembler paymentMaker;
        PaymentRepository paymentRepo;
        TransactionService transactionService;
        TransactionDtoAssembler _transactionDtoMaker;
        private readonly LedgerSetupRepository _ledgerSetupRepo;

        public PaymentServiceImpl(TransactionDtoAssembler transactionDtoMaker, TransactionService _transactionService, PaymentAssembler _paymentMaker, PaymentRepository _paymentRepo, LedgerSetupRepository ledgerSetupRepo)
        {
            transactionService = _transactionService;
            paymentMaker = _paymentMaker;
            paymentRepo = _paymentRepo;
            _transactionDtoMaker = transactionDtoMaker;
            _ledgerSetupRepo = ledgerSetupRepo;
        }

        public void cancel(long payment_id, long user_id)
        {
            // P1 fix: ambient TransactionScope was a no-op for EF Core; the reverse
            // ledger entry + payment update now run in one real database transaction.
            using (var tx = paymentRepo.beginTransaction())
            {
                var payment = paymentRepo.getById(payment_id);
                if (payment == null)
                {
                    throw new ItemNotFoundException("Payment doesnot exists.");
                }
                if (payment.is_cancelled == true)
                {
                    throw new ItemUsedException("Payment is already cancelled.");
                }
                TransactionDto transactionDto = getTransactionDtoForReverseEntry(payment);
                transactionService.addTransaction(transactionDto);

                payment.is_cancelled = true;
                payment.cancelled_by = user_id;
                payment.cancelled_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
                paymentRepo.update(payment);
                paymentRepo.saveChanges();
                tx.Commit();
            }
        }

        private TransactionDto getTransactionDtoForReverseEntry(Payment payment)
        {
            TransactionDto transactionDto = new TransactionDto();
            transactionDto.transaction_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
            transactionDto.voucher_no = payment.payment_id;
            transactionDto.voucher_type = VoucherType.Payment;
            transactionDto.remarks = "Being Payment Cancelled";

            // P1/B11 fix: this method mutated 'debitTransactionDetailDto' after adding it
            // to the transaction (the discount branch overwrote its ledger/amount), so
            // the original debit line vanished and Dr ≠ Cr. Each line is now its own
            // separate DTO instance.
            LedgerTransactionDto creditTransactionDetailDto = new LedgerTransactionDto();
            creditTransactionDetailDto.ledger_id = payment.payment_to;
            creditTransactionDetailDto.amount = payment.amount + payment.discount;
            transactionDto.addCreditData(creditTransactionDetailDto);

            LedgerTransactionDto debitTransactionDetailDto = new LedgerTransactionDto();
            debitTransactionDetailDto.ledger_id = payment.payment_from;
            debitTransactionDetailDto.amount = payment.amount;
            transactionDto.addDebitData(debitTransactionDetailDto);

            if (payment.discount > 0)
            {
                LedgerTransactionDto discountDebitDetailDto = new LedgerTransactionDto();
                //check whether settings is available or not
                Entities.LedgerSetup discount_setting = _ledgerSetupRepo.getByKey(LE.Account.Common.Enums.LedgerSetup.discount_received.ToString());
                if (discount_setting == null)
                    throw new ItemNotFoundException("No setup found for discount received.");
                discountDebitDetailDto.ledger_id = Convert.ToInt32(discount_setting.value);
                discountDebitDetailDto.amount = payment.discount;
                transactionDto.addDebitData(discountDebitDetailDto);
            }

            return transactionDto;
        }

        public void doPayment(PaymentDto paymentDto)
        {
            using (var tx = paymentRepo.beginTransaction())
            {
                if (!paymentDto.isValid())
                    throw new InvalidValueException("The provided data are not valid.");
                Payment payment = new Payment();
                paymentMaker.copy(payment, paymentDto);
                paymentRepo.insert(payment);
                paymentDto.payment_id = payment.payment_id;
                TransactionDto transactionDto = _transactionDtoMaker.createTransactionDtoFrom(paymentDto);

                transactionService.addTransaction(transactionDto);
                paymentRepo.saveChanges();
                tx.Commit();
            }
        }
    }
}
