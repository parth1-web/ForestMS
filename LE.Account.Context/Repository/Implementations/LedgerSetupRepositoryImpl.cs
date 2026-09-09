using LE.Account.Repository.Interface;
using System.Linq;
using LE.Entities.Account;
using LE.Context.Data;
using LE.Common.Repository.Implementations;

namespace LE.Account.Context.Repository.Implementations
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
