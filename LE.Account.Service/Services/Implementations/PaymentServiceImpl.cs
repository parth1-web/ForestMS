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
            using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
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
                tx.Complete();
            }


        }

        private TransactionDto getTransactionDtoForReverseEntry(Payment payment)
        {
            TransactionDto transactionDto = new TransactionDto();
            transactionDto.transaction_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
            transactionDto.voucher_no = payment.payment_id;
            transactionDto.voucher_type = VoucherType.Payment;
            transactionDto.remarks = "Being Payment Cancelled";
            LedgerTransactionDto debitTransactionDetailDto = new LedgerTransactionDto();
            LedgerTransactionDto creditTransactionDetailDto = new LedgerTransactionDto();

            creditTransactionDetailDto.ledger_id = payment.payment_to;
            creditTransactionDetailDto.amount = payment.amount + payment.discount;
            transactionDto.addCreditData(creditTransactionDetailDto);

            debitTransactionDetailDto.ledger_id = payment.payment_from;
            debitTransactionDetailDto.amount = payment.amount;
            transactionDto.addDebitData(debitTransactionDetailDto);

            if (payment.discount > 0)
            {
                LedgerTransactionDto crTransactionDetailDto = new LedgerTransactionDto();
                //check whether settings is available or not
                Entities.LedgerSetup discount_setting = _ledgerSetupRepo.getByKey(LE.Account.Common.Enums.LedgerSetup.discount_received.ToString());
                if (discount_setting == null)
                    throw new ItemNotFoundException("No setup found for discount received.");
                debitTransactionDetailDto.ledger_id = Convert.ToInt32(discount_setting.value);
                debitTransactionDetailDto.amount = payment.discount;
                transactionDto.addDebitData(debitTransactionDetailDto);
            }

            return transactionDto;
        }

        public void doPayment(PaymentDto paymentDto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    if (!paymentDto.isValid())
                        throw new InvalidValueException("The provided data are not valid.");
                    Payment payment = new Payment();
                    paymentMaker.copy(payment, paymentDto);
                    paymentRepo.insert(payment);
                    paymentDto.payment_id = payment.payment_id;
                    TransactionDto transactionDto = _transactionDtoMaker.createTransactionDtoFrom(paymentDto);

                    transactionService.addTransaction(transactionDto);
                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
