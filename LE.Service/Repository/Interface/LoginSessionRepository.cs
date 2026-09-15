using LE.Entities.User;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;

namespace LE.Service.Repository.Interface
{
    public interface LoginSessionRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(LoginSession login_session);
        List<LoginSession> getAll();
        IQueryable<LoginSession> getQueryable();
    }
}
