using DateConverter.Core.Service_Factory;
using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using LE.Account.Service.Assemblers.Interface;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Account.Service.Assemblers.Implementations
{
    public class LedgerAssemblerImpl : LedgerAssembler
    {
        public void copy(Ledger ledger, LedgerDto ledger_dto)
        {
            ledger.ledger_id = ledger_dto.ledger_id;
            ledger.name = ledger_dto.name;
            ledger.created_date = ledger_dto.created_date;
            ledger.ledger_group_id = ledger_dto.ledger_group_id;
            ledger.code = ledger_dto.code;
            ledger.user_id = ledger_dto.user_id;
            var dateConverterService = DateConverterFactory.getDateConverterService();
            ledger.nep_created_date = dateConverterService.ToBS(ledger_dto.created_date.Date, DateFormats.mDy).getFormattedDate().ToString();
        }
    }
}
