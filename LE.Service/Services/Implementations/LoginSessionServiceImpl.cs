using LE.Common.Exceptions;
using LE.Entities.User;
using LE.Infrastructure.Dto;
using LE.Service.Assembler.Interface;
using LE.Service.Repository.Interface;
using LE.Service.Services.Interface;
using System.Transactions;

namespace LE.Service.Services.Implementations
{
    public class LoginSessionServiceImpl : LoginSessionService
    {
        private readonly LoginSessionMaker _loginSessionMaker;
        private readonly LoginSessionRepository _loginSessionRepo;
        private readonly AuthenticationRepository _authenticationRepo;

        public LoginSessionServiceImpl(LoginSessionMaker loginSessionMaker, LoginSessionRepository loginSessionRepo, AuthenticationRepository authenticationRepo)
        {
            _loginSessionMaker = loginSessionMaker;
            _loginSessionRepo = loginSessionRepo;
            _authenticationRepo = authenticationRepo;
        }

        public void save(LoginSessionDto session_dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var authentication = _authenticationRepo.getById(session_dto.authentication_id);
                    LoginSession sessionDetail = new LoginSession();
                    _loginSessionMaker.copy(sessionDetail, session_dto);

                    sessionDetail.authentication = authentication ?? throw new ItemNotFoundException($"Authentication with the id {session_dto.authentication_id} doesnot exist.");

                    _loginSessionRepo.insert(sessionDetail);
                    tx.Complete();
                }
            }
            catch (System.Exception)
            {
                throw;
            }

        }
    }
}
