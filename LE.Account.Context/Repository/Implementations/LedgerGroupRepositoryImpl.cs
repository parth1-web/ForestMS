using LE.Account.Repository.Interface;
using LE.Entities.Account;
using LE.Context.Data;
using LE.Common.Repository.Implementations;
using System.Collections.Generic;
using System.Linq;
using LE.Account.Common.Enums;
using System;

namespace LE.Account.Context.Repository.Implementations
{
    public class LedgerGroupRepositoryImpl:BaseRepositoryImpl<LedgerGroup>,LedgerGroupRepository
    {
        private readonly AppDbContext AppDbContext;
        public LedgerGroupRepositoryImpl(AppDbContext _AppDbContext) : base(_AppDbContext)
        {
            AppDbContext = _AppDbContext;

        }

        public LedgerGroup getByName(string name)
        {
            return AppDbContext.ledger_group.Where(a => a.name == name).SingleOrDefault();
        }

        public List<LedgerGroup> getLedgerGroupByAccountType(LedgerGroupType account_type_id)
        {
            return AppDbContext.ledger_group.Where(a => a.ledger_group_type ==account_type_id ).ToList();
        }

        //public List<Ledger> getLedgersUnderCashGroup()
        //{
        //    var ledgerGroup = AppDbContext.ledger_group.Where(a => a.code == "1.6").SingleOrDefault();
        //    return ledgerGroup.ledgers;
        //}

        //public List<Ledger> getLedgersUnderPurchaseGroup()
        //{
        //    var ledgerGroup = AppDbContext.ledger_group.Where(a => a.code == "4.4").SingleOrDefault();
        //    return ledgerGroup.ledgers;
        //}

        //public List<Ledger> getLedgersUnderPurchaseReturnGroup()
        //{
        //    var ledgerGroup = AppDbContext.ledger_group.Where(a => a.code == "3.5").SingleOrDefault();
        //    return ledgerGroup.ledgers;
        //}

        //public List<Ledger> getLedgersUnderSalesReturnGroup()
        //{
        //    var ledgerGroup = AppDbContext.ledger_group.Where(a => a.code == "4.6").SingleOrDefault();
        //    return ledgerGroup.ledgers;
        //}

        //public List<Ledger> getLedgersUnderSalesGroup()
        //{
        //    var ledgerGroup = AppDbContext.ledger_group.Where(a => a.code == "3.6").SingleOrDefault();
        //    return ledgerGroup.ledgers;
        //}

        //public List<Ledger> getLedgersUnderDiscountAllowedGroup()
        //{
        //    var ledgerGroup = AppDbContext.ledger_group.Where(a => a.code == "4.5").SingleOrDefault();
        //    return ledgerGroup.ledgers;
        //}

        //public List<Ledger> getLedgersUnderDiscountReceivedGroup()
        //{
        //    var ledgerGroup = AppDbContext.ledger_group.Where(a => a.code == "3.4").SingleOrDefault();
        //    return ledgerGroup.ledgers;
        //}

        //public List<Ledger> getLedgersUnderBankGroup()
        //{
        //    var ledgerGroup = AppDbContext.ledger_group.Where(a => a.code == "1.4").ToList().SingleOrDefault();
        //    return ledgerGroup.ledgers;
        //}

        //public List<Ledger> getLedgersUnderCashAndBankGroup()
        //{
        //    var ledgerGroup = AppDbContext.ledger_group.Where(a => a.code == "1.4" || a.code=="1.6").ToList().SingleOrDefault();
        //    return ledgerGroup.ledgers;
        //}

        //public List<Ledger> getLedgersUnderLiabilityGroup()
        //{
        //    var ledgerGroup = AppDbContext.ledger_group.Where(a => a.code == "2.3").ToList().SingleOrDefault();
        //    return ledgerGroup.ledgers;
        //}

        public LedgerGroup getByCode(string code)
        {
            return AppDbContext.ledger_group.Where(a => a.code == code).SingleOrDefault();
        }
    }
}
