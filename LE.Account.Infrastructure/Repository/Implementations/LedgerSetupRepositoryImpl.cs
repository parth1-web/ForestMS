using LE.Account.Entities;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System.Linq;

namespace LE.Account.Infrastructure.Repository.Implementations
{
    public class LedgerSetupRepositoryImpl : BaseRepositoryImpl<LedgerSetup>, LedgerSetupRepository
    {
        private readonly AppDbContext _accontDbContext;

        public LedgerSetupRepositoryImpl(AppDbContext context) : base(context)
        {
            _accontDbContext = context;
        }

        public LedgerSetup getByKey(string key)
        {
            return _accontDbContext.ledger_setup.Where(a => a.key == key).SingleOrDefault();
        }
    }
}
