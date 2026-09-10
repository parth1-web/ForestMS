using LE.Common.Enums;
using LE.Common.Exceptions;
using LE.Common.Library;
using LE.Entities.User;
using LE.Infrastructure.Dto;
using LE.Service.Assembler.Interface;
using LE.Service.Repository.Interface;
using LE.Service.Services.Interface;
using System;
using System.Transactions;

namespace LE.Service.Services.Implementations
{
    public class AuthenticationServiceImpl : AuthenticationService
    {
        private readonly AuthenticationRepository _authenticationRepo;
        private readonly AuthenticationMaker _authenticationMaker;
        private readonly PasswordHash _passwordHash;

        public AuthenticationServiceImpl(AuthenticationRepository authenticationRepo, EncryptDecrypt encryptDecrypt, AuthenticationMaker authenticationMaker, PasswordHash passwordHash)
        {
            _passwordHash = passwordHash;
            _authenticationRepo = authenticationRepo;
            _authenticationMaker = authenticationMaker;
        }

        public void disable(long type_id, UserType type = UserType.user)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {

                    var authentication = _authenticationRepo.getByType(type_id, type) ?? throw new ItemNotFoundException($"Authentication detail doesnot exist.");

                    authentication.deactivate();
                    _authenticationRepo.update(authentication);
                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void enable(long type_id, UserType type = UserType.user)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var authentication = _authenticationRepo.getByType(type_id, type) ?? throw new ItemNotFoundException($"Authentication detail doesnot exist.");

                    authentication.activate();
                    _authenticationRepo.update(authentication);
                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void save(AuthenticationDto authentication_dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var authentication = _authenticationRepo.getByType(authentication_dto.type_id, authentication_dto.type);

                    if (authentication != null)
                    {
                        throw new DuplicateItemException("Authentication for specified person already exists.");
                    }

                    var authenticationWithSameName = _authenticationRepo.getByUsername(authentication_dto.username);

                    if (authenticationWithSameName != null)
                    {
                        throw new DuplicateItemException("Authentication with same username already exists.");
                    }

                    authentication = new Authentication();

                    _authenticationMaker.copy(authentication, authentication_dto);

                    authentication.password = _passwordHash.CreateHash(authentication_dto.password);

                    _authenticationRepo.insert(authentication);

                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void updatePassword(UpdatePasswordDto dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var authentication = _authenticationRepo.getByType(dto.type_id, dto.type);

                    bool isOldPasswordCorrect = _passwordHash.ValidatePassword(dto.old_password, authentication.password);

                    if (!isOldPasswordCorrect)
                    {
                        throw new InvalidValueException($"Old password is incorrect.");
                    }

                    authentication.password = _passwordHash.CreateHash(dto.new_password);

                    _authenticationRepo.update(authentication);

                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        public void updateUsername(string new_name, long type_id, UserType type = UserType.user)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var authentication = _authenticationRepo.getByType(type_id, type) ?? throw new ItemNotFoundException($"Authentication detail doesnot exist.");

                    var authenticationWithSameName = _authenticationRepo.getByUsername(new_name);

                    bool isUsernameAllowed = authenticationWithSameName == null || authenticationWithSameName.authentication_id == authentication.authentication_id;

                    if (!isUsernameAllowed)
                    {
                        throw new DuplicateItemException("Authentication with same username already exists.");
                    }

                    authentication.username = new_name;
                    _authenticationRepo.update(authentication);

                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Authentication validateUser(string username, string password)
        {
            var authentication = _authenticationRepo.getByUsername(username);
            if (authentication == null)
            {
                return null;
            }
            if (!authentication.is_enabled)
            {
                // Disabled accounts must never authenticate, regardless of a correct password.
                return null;
            }
            if (!_passwordHash.ValidatePassword(password, authentication.password))
            {
                return null;
            }

            // Upgrade a legacy (low-iteration) stored hash to the current format.
            // The password itself is unchanged; only its stored representation is strengthened.
            if (_passwordHash.NeedsRehash(authentication.password))
            {
                authentication.password = _passwordHash.CreateHash(password);
                _authenticationRepo.update(authentication);
            }

            return authentication;
        }
    }
}
