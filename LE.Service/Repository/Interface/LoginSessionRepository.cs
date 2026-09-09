using LE.Entities.User;
using System.Collections.Generic;
using System.Linq;

namespace LE.Service.Repository.Interface
{
    public interface LoginSessionRepository
    {
        void insert(LoginSession login_session);
        List<LoginSession> getAll();
        IQueryable<LoginSession> getQueryable();
    }
}
