using LE.Account.Common.Enums;
using LE.Account.Entities;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System.Collections.Generic;
using System.Linq;

namespace LE.Account.Infrastructure.Repository.Implementations
{
    public class LedgerRepositoryImpl : BaseRepositoryImpl<Ledger>, LedgerRepository
    {
        private readonly AppDbContext AppDbContext;
        public LedgerRepositoryImpl(AppDbContext _AppDbContext) : base(_AppDbContext)
        {
            AppDbContext = _AppDbContext;
        }

        public Ledger getByName(string name)
        {
            return AppDbContext.ledger.Where(a => a.name == name).SingleOrDefault();
        }

        public List<Ledger> getLedgersByLedgerGroup(long ledger_group_id)
        {
            return AppDbContext.ledger.Where(a => a.ledger_group_id == ledger_group_id).ToList();
        }


        public List<Ledger> getLedgersUnderAssetsGroup()
        {
            var assetsGroup = AppDbContext.ledger.Where(a => a.ledger_group.ledger_group_type == LedgerGroupType.asset).ToList();
            return assetsGroup;
        }

        public List<Ledger> getLedgersUnderIncomeGroup()
        {
            var incomeGroup = AppDbContext.ledger.Where(a => a.ledger_group.ledger_group_type == LedgerGroupType.income).ToList();
            return incomeGroup;
        }

        public List<Ledger> getLedgersUnderExpensesGroup()
        {
            var expensesGroup = AppDbContext.ledger.Where(a => a.ledger_group.ledger_group_type == LedgerGroupType.expenses).ToList();
            return expensesGroup;
        }
        public List<Ledger> getLedgersUnderLiabilitiesGroup()
        {
            var liabilitiesGroup = AppDbContext.ledger.Where(a => a.ledger_group.ledger_group_type == LedgerGroupType.liability).ToList();
            return liabilitiesGroup;
        }

    }
}
