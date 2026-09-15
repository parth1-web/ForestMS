using LE.Common.Exceptions;
using LE.Entities.User;
using LE.Infrastructure.Dto;
using LE.Service.Repository.Interface;
using LE.Service.Services.Interface;
using System;
using System.Linq;

namespace LE.Service.Services.Implementations
{
    public class UserRoleServiceImpl : UserRoleService
    {
        private readonly UserRoleRepository _userRoleRepo;
        private readonly RoleRepository _roleRepo;

        public UserRoleServiceImpl(UserRoleRepository userRoleRepo, RoleRepository roleRepo)
        {
            _userRoleRepo = userRoleRepo;
            _roleRepo = roleRepo;
        }

        public void save(UserRoleDto dto)
        {
            try
            {
                using (var tx = _userRoleRepo.beginTransaction())
                {
                    var previousAssignedRoles = _userRoleRepo.getByTypeId(dto.type, dto.type_id);

                bool isRoleAlreadyAssignedToUser = previousAssignedRoles.Count > 0;

                if (isRoleAlreadyAssignedToUser)
                {
                    throw new InvalidValueException("Roles have already been assigned to the user.");
                }

                    foreach (var roleId in dto.role_ids.Distinct())
                    {
                        UserRole user_role = new UserRole();
                        user_role.role_id = roleId;
                        user_role.role = _roleRepo.getById(roleId) ?? throw new ItemNotFoundException($"Role with the role id {roleId} doesnot exist.");

                        user_role.type = dto.type;
                        user_role.type_id = dto.type_id;
                        _userRoleRepo.insert(user_role);
                    }
                    _userRoleRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void update(UserRoleDto dto)
        {
            try
            {
                using (var tx = _userRoleRepo.beginTransaction())
                {
                    var previousAssignedRoles = _userRoleRepo.getByTypeId(dto.type, dto.type_id);

                    var removedUserRoles = previousAssignedRoles.Where(l1 => !dto.role_ids.Any(role_id => l1.role_id == role_id)).ToList();

                    foreach (var removedUserRole in removedUserRoles)
                    {
                        _userRoleRepo.delete(removedUserRole);
                    }

                    var addedRoleIds = dto.role_ids.Where(l1 => !previousAssignedRoles.Any(user_roles => l1 == user_roles.role_id)).ToList();

                    foreach (var roleId in addedRoleIds.Distinct())
                    {
                        UserRole user_role = new UserRole();
                        user_role.role_id = roleId;
                        user_role.role = _roleRepo.getById(roleId) ?? throw new ItemNotFoundException($"Role with the role id {roleId} doesnot exist.");

                        user_role.type = dto.type;
                        user_role.type_id = dto.type_id;
                        _userRoleRepo.insert(user_role);
                    }
                    _userRoleRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
