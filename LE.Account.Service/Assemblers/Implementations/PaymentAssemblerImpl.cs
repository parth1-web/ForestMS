using DateConverter.Core.Service_Factory;
using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using LE.Account.Service.Assemblers.Interface;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Account.Service.Assemblers.Implementations
{
    public class PaymentAssemblerImpl : PaymentAssembler
    {
        public void copy(Payment payment, PaymentDto payment_dto)
        {
            payment.transaction_date = payment_dto.transaction_date;
            payment.remarks = payment_dto.remarks;
            payment.payment_from = payment_dto.payment_from;
            payment.payment_to = payment_dto.payment_to;
            payment.amount = payment_dto.amount;
            payment.discount = payment_dto.discount;
            payment.user_id = payment_dto.user_id;
            var dateConverterService = DateConverterFactory.getDateConverterService();
            payment.nep_entry_date = dateConverterService.ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate().ToString();
            payment.nep_transaction_date = dateConverterService.ToBS(payment_dto.transaction_date.Date, DateFormats.yMd).getFormattedDate().ToString();

        }
    }
}
