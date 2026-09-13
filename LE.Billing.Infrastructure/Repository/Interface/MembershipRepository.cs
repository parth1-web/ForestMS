using LE.Billing.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface MembershipRepository
    {
        // Real transaction boundary on the shared AppDbContext (see BaseRepositoryImpl).
        IDbContextTransaction beginTransaction();
        void insert(Membership membership);
        void update(Membership membership);
        List<Membership> getAll();
        Membership getById(long membershipId);
        IQueryable<Membership> getQueryable();
        Membership GetByCode(string code);
    }
}
