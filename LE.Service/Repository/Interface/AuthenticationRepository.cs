using LE.Common.Enums;
using LE.Entities.User;
using System.Collections.Generic;
using System.Linq;

namespace LE.Service.Repository.Interface
{
    public interface AuthenticationRepository
    {
        void insert(Authentication authentication);
        void update(Authentication authentication);
        List<Authentication> getAll();
        Authentication getById(long authentication_id);
        Authentication getByUsername(string username);
        Authentication getByType(long type_id,UserType type);
        IQueryable<Authentication> getQueryable();
    }
}
