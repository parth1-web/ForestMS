using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class ToleRepositoryImpl : BaseRepositoryImpl<Tole>, ToleRepository
    {
        private readonly AppDbContext _appDbContext;

        public ToleRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public Tole getByToleNo(string tole_no)
        {
            return _appDbContext.tole.Where(a => a.tole_no == tole_no).SingleOrDefault();
        }
    }
}
