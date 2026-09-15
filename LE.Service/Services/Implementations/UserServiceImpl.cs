using LE.Common.Exceptions;
using LE.Infrastructure.Dto;
using LE.Service.Assembler.Interface;
using LE.Service.Repository.Interface;
using LE.Service.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace LE.Service.Services.Implementations
{
    using userEntity = LE.Entities.User.User;
    public class UserServiceImpl : UserService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserRepository _userRepo;
        private readonly RoleRepository _userRoleRepo;
        private readonly AuthenticationRepository _authenticationRepo;
        private readonly AuthenticationService _authenticationService;
        private readonly UserMaker _userMaker;
        private readonly UserRoleService _userRoleService;


        public UserServiceImpl(UserRepository userRepo, RoleRepository userRoleRepo, AuthenticationRepository authenticationRepo, AuthenticationService authenticationService, UserMaker userMaker, UserRoleService userRoleService, IHostingEnvironment hostingEnvironment)
        {
            _userRepo = userRepo;
            _userRoleRepo = userRoleRepo;
            _authenticationRepo = authenticationRepo;
            _authenticationService = authenticationService;
            _userMaker = userMaker;
            _userRoleService = userRoleService;
            _hostingEnvironment = hostingEnvironment;
        }

        public void disable(long user_id)
        {
            try
            {
                using (var tx = _userRepo.beginTransaction())
                {
                    var user = _userRepo.getById(user_id) ?? throw new ItemNotFoundException($"User with the id {user_id} doesnot exist.");

                    user.disable();
                    _userRepo.update(user);

                    _authenticationService.disable(user_id, LE.Common.Enums.UserType.user);
                    _userRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void enable(long user_id)
        {
            try
            {
                using (var tx = _userRepo.beginTransaction())
                {
                    var user = _userRepo.getById(user_id) ?? throw new ItemNotFoundException($"User with the id {user_id} doesnot exist.");

                    user.enable();
                    _userRepo.update(user);

                    _authenticationService.enable(user_id, LE.Common.Enums.UserType.user);
                    _userRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void save(UserDto user_dto)
        {
            try
            {
                using (var tx = _userRepo.beginTransaction())
                {
                    if (user_dto.role_ids.Count == 0)
                    {
                        throw new InvalidValueException("At least one role must be specified.");
                    }

                    userEntity user = new userEntity();
                    _userMaker.copy(user, user_dto);
                    user.created_date = DateTime.Now;
                    _userRepo.insert(user);

                    UserRoleDto userRoleDto = new UserRoleDto()
                    {
                        type = LE.Common.Enums.UserType.user,
                        type_id = user.user_id,
                        role_ids = user_dto.role_ids.ToArray()
                    };

                    _userRoleService.save(userRoleDto);

                    AuthenticationDto authenticationDto = new AuthenticationDto()
                    {
                        username = user_dto.username,
                        password = user_dto.password,
                        type = LE.Common.Enums.UserType.user,
                        type_id = user.user_id
                    };

                    _authenticationService.save(authenticationDto);
                    _userRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void update(UserDto user_dto)
        {
            try
            {
                using (var tx = _userRepo.beginTransaction())
                {
                    if (user_dto.role_ids.Count == 0)
                    {
                        throw new InvalidValueException("At least one role must be specified.");
                    }

                    userEntity user = _userRepo.getById(user_dto.user_id) ?? throw new ItemNotFoundException($"User with the id {user_dto.user_id} doesnot exist.");

                    if (!string.IsNullOrWhiteSpace(user_dto.image_path))
                    {
                        if (!string.IsNullOrWhiteSpace(user.image_path))
                        {
                            deleteImage(user.image_path);
                        }
                    }

                    _userMaker.copy(user, user_dto);
                    _userRepo.update(user);
                    _userRoleService.update(new UserRoleDto()
                    {
                        type = LE.Common.Enums.UserType.user,
                        type_id = user.user_id,
                        role_ids = user_dto.role_ids.ToArray()
                    });

                    _authenticationService.updateUsername(user_dto.username, user_dto.user_id);
                    _userRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected void deleteImage(string image_path)
        {
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images/custom");
            if (File.Exists(Path.Combine(filePath, image_path)))
            {
                File.Delete(Path.Combine(filePath, image_path));
            }
        }
    }
}
