using LE.Common.Exceptions;
using LE.Entities.User;
using LE.Infrastructure.Dto;
using LE.Service.Repository.Interface;
using LE.Service.Services.Interface;
using System;
using System.Linq;
using System.Transactions;

namespace LE.Service.Services.Implementations
{
    public class RoleServiceImpl : RoleService
    {
        private readonly RoleRepository _roleRepo;
        private readonly RolePermissionMapService _rolePermissionMapService;
        private readonly UserRoleRepository _userRoleRepo;

        public RoleServiceImpl(RoleRepository roleRepo, RolePermissionMapService rolePermissionMapService, UserRoleRepository userRoleRepo)
        {
            _roleRepo = roleRepo;
            _rolePermissionMapService = rolePermissionMapService;
            _userRoleRepo = userRoleRepo;

        }

        public void delete(long role_id)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var role = _roleRepo.getById(role_id);

                    if (role == null)
                    {
                        throw new ItemNotFoundException($"User Role with the id {role_id} doesnot exist.");
                    }

                    var usersWithSpecifiedRole = _userRoleRepo.getByRoleId(role_id);

                    bool usersWithRoleExists = usersWithSpecifiedRole.Count > 0;
                    if (usersWithRoleExists)
                    {
                        throw new ItemUsedException("The specified role has already been assigned to some users.");
                    }
                    _roleRepo.delete(role);
                    _rolePermissionMapService.deletePermissionsByRoleId(role_id);
                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void disable(long role_id)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var role = _roleRepo.getById(role_id);

                    if (role == null)
                    {
                        throw new ItemNotFoundException($"User Role with the id {role_id} doesnot exist.");
                    }
                    role.disable();
                    _roleRepo.update(role);
                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void enable(long role_id)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var role = _roleRepo.getById(role_id);

                    if (role == null)
                    {
                        throw new ItemNotFoundException($"User Role with the id {role_id} doesnot exist.");
                    }
                    role.enable();

                    _roleRepo.update(role);
                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public void save(RoleDto role_dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var roleWithSameName = _roleRepo.getByName(role_dto.name.Trim());

                    if (roleWithSameName != null)
                    {
                        throw new DuplicateItemException($"User Role with same name already exists.");
                    }

                    Role role = new Role()
                    {
                        name = role_dto.name,
                        is_enabled = role_dto.is_active
                    };
                    _roleRepo.insert(role);

                    var rolePermissionDto = new RolePermissionMapDto();
                    rolePermissionDto.role_id = role.role_id;
                    foreach (var permission in role_dto.module_ids)
                    {
                        rolePermissionDto.addPermission(permission);
                    }
                    _rolePermissionMapService.saveOrUpdate(rolePermissionDto);

                    tx.Complete();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void update(RoleDto role_dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var roleWithSameName = _roleRepo.getByName(role_dto.name.Trim());

                    if (roleWithSameName != null && roleWithSameName.role_id != role_dto.role_id)
                    {
                        throw new DuplicateItemException($"User Role with same name already exists.");
                    }

                    roleWithSameName.is_enabled = role_dto.is_active;
                    roleWithSameName.name = role_dto.name;
                    _roleRepo.update(roleWithSameName);

                    updateRolePermissions(role_dto);

                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        private void updateRolePermissions(RoleDto role_dto)
        {
            RolePermissionMapDto rolePermissionMapDto = new RolePermissionMapDto();
            rolePermissionMapDto.role_id = role_dto.role_id;
            foreach (var permission in role_dto.module_ids.Distinct())
            {
                rolePermissionMapDto.addPermission(permission);
            }
            _rolePermissionMapService.saveOrUpdate(rolePermissionMapDto);
        }
    }
}
