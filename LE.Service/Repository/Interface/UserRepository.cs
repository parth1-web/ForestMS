using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage;

namespace LE.Service.Repository.Interface
{
    using userEntity = LE.Entities.User.User;

    public interface UserRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(userEntity user);
        void update(userEntity user);
        List<userEntity> getAll();
        userEntity getById(long user_id);
        List<userEntity> getByName(string user_name);
        IQueryable<userEntity> getQueryable();
    }
}
