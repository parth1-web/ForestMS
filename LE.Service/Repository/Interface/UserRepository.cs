using System.Collections.Generic;
using System.Linq;

namespace LE.Service.Repository.Interface
{
    using userEntity = LE.Entities.User.User;

    public interface UserRepository
    {
        void insert(userEntity user);
        void update(userEntity user);
        List<userEntity> getAll();
        userEntity getById(long user_id);
        List<userEntity> getByName(string user_name);
        IQueryable<userEntity> getQueryable();
    }
}
