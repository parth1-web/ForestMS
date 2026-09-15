using LE.Account.Common.Enums;
using LE.Account.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;

namespace LE.Account.Infrastructure.Repository.Interface
{
    public interface LedgerGroupRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(LedgerGroup ledger_group);
        void update(LedgerGroup ledger_group);
        void delete(LedgerGroup ledger_group);
        List<LedgerGroup> getAll();
        List<LedgerGroup> getLedgerGroupByAccountType(LedgerGroupType account_type_id);
        LedgerGroup getById(long ledger_group_id);
        LedgerGroup getByCode(string code);
        LedgerGroup getByName(string name);
        IQueryable<LedgerGroup> getQueryable();
        //List<Ledger> getLedgersUnderBankGroup();
        //List<Ledger> getLedgersUnderCashGroup();
        //List<Ledger> getLedgersUnderCashAndBankGroup();
        //List<Ledger> getLedgersUnderSalesGroup();
        //List<Ledger> getLedgersUnderPurchaseGroup();
        //List<Ledger> getLedgersUnderPurchaseReturnGroup();
        //List<Ledger> getLedgersUnderSalesReturnGroup();
        //List<Ledger> getLedgersUnderDiscountAllowedGroup();
        //List<Ledger> getLedgersUnderDiscountReceivedGroup();
        //List<Ledger> getLedgersUnderLiabilityGroup();
    }
}
