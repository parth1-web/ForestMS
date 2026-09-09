using DateConverter.Core.Service_Factory;
using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using LE.Account.Service.Assemblers.Interface;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Account.Service.Assemblers.Implementations
{
    public class ReceiptAssemblerImpl : ReceiptAssembler
    {
        public void copy(Receipt receipt, ReceiptDto receipt_dto)
        {
            receipt.receipt_from = receipt_dto.receipt_from;
            receipt.receipt_to = receipt_dto.receipt_to;
            receipt.remarks = receipt_dto.remarks;
            receipt.amount = receipt_dto.amount;
            receipt.discount = receipt_dto.discount;
            receipt.user_id = receipt_dto.user_id;
            receipt.customer_name = receipt_dto.customer_name;
            receipt.transaction_date = receipt_dto.transaction_date;
            var dateConverterService = DateConverterFactory.getDateConverterService();
            receipt.nep_entry_date = dateConverterService.ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate().ToString();
            receipt.nep_transaction_date = dateConverterService.ToBS(receipt_dto.transaction_date.Date, DateFormats.yMd).getFormattedDate().ToString();

        }
    }
}
