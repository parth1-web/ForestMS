using LE.Billing.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface MembershipValidityRepository
    {
        void insert(MembershipValidity membershipPeriod);
        void update(MembershipValidity membershipPeriod);
        List<MembershipValidity> getAll();
        MembershipValidity getById(long membership_id);
        IQueryable<MembershipValidity> getQueryable();
    }
}
