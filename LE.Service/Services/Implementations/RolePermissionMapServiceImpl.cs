using LE.Common.Exceptions;
using LE.Entities.User;
using LE.Infrastructure.Dto;
using LE.Service.Repository.Interface;
using LE.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Service.Services.Implementations
{
    public class RolePermissionMapServiceImpl : RolePermissionMapService
    {
        private readonly RolePermissionMapRepository _rolePermissionMapRepo;
        private readonly RoleRepository _roleRepo;
        private readonly ModuleRepository _moduleRepo;

        public RolePermissionMapServiceImpl(RolePermissionMapRepository rolePermissionMapRepo, RoleRepository roleRepo, ModuleRepository moduleRepo)
        {
            _rolePermissionMapRepo = rolePermissionMapRepo;
            _roleRepo = roleRepo;
            _moduleRepo = moduleRepo;
        }

        public void deletePermissionsByRoleId(long role_id)
        {
            var permissions = _rolePermissionMapRepo.getByRoleId(role_id);
            foreach (var permission in permissions)
            {
                _rolePermissionMapRepo.delete(permission);
            }
        }

        public void saveOrUpdate(RolePermissionMapDto dto)
        {
            try
            {
                using (var tx = _rolePermissionMapRepo.beginTransaction())
                {
                    if (dto.module_ids.Count == 0)
                    {
                        throw new InvalidValueException("At least one permission is required.");
                    }

                    var previousSavePermissions = _rolePermissionMapRepo.getByRoleId(dto.role_id);
                    if (previousSavePermissions.Count == 0)
                    {
                        save(dto);
                    }
                    else
                    {
                        update(dto);
                    }
                    _rolePermissionMapRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected void save(RolePermissionMapDto dto)
        {
            if (dto.module_ids.Count == 0)
            {
                throw new InvalidValueException("At least one permission is required.");
            }

            var role = _roleRepo.getById(dto.role_id) ?? throw new ItemNotFoundException($"User role with the id {dto.role_id} doesnot exist.");

            foreach (var moduleId in dto.module_ids)
            {
                RolePermissionMap rolePermissionMap = new RolePermissionMap()
                {
                    role = role,
                    module_id = moduleId,
                    module = _moduleRepo.getById(moduleId) ?? throw new ItemNotFoundException($"Module with id {moduleId} doesnot exist.")
                };
                _rolePermissionMapRepo.insert(rolePermissionMap);
            }
        }

        protected void update(RolePermissionMapDto dto)
        {
            var previousSavePermissions = _rolePermissionMapRepo.getByRoleId(dto.role_id) ?? throw new ItemNotFoundException($"Permissions has not been assigned to specified role.");

            List<long> previouslyAssignedPermissions = previousSavePermissions.Select(a => a.module_id).ToList();

            var userRole = _roleRepo.getById(dto.role_id) ?? throw new ItemNotFoundException($"User role with the id {dto.role_id} doesnot exist.");

            var removedPermissions = previousSavePermissions.Where(l1 => !dto.module_ids.Any(permission => l1.module_id == permission)).ToList();

            foreach (var rolePermissionMap in removedPermissions)
            {
                _rolePermissionMapRepo.delete(rolePermissionMap);
            }

            var addedPermissions = dto.module_ids.Where(l1 => !previousSavePermissions.Any(permission => l1 == permission.module_id)).ToList();

            dto.removeAllPermissions();

            foreach (var addedPermission in addedPermissions)
            {
                dto.addPermission(addedPermission);
            }

            if (dto.module_ids.Count > 0)
            {
                save(dto);
            }
        }
    }
}
