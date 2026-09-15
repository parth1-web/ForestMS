using LE.Billing.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface MemberRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(Member member);
        void update(Member member);
        List<Member> getAll();
        Member getById(long memberId);
        Member getByName(string memberName);
        IQueryable<Member> getQueryable();
	}
}
