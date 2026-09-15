using LE.Common.Exceptions;
using LE.Entities.User;
using LE.Infrastructure.Dto;
using LE.Service.Assembler.Interface;
using LE.Service.Repository.Interface;
using LE.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Service.Services.Implementations
{
    public class ModuleServiceImpl : ModuleService
    {
        private readonly ModuleRepository _moduleRepo;
        private readonly ModuleAssembler _moduleAssembler;

        public ModuleServiceImpl(ModuleRepository moduleRepo, ModuleAssembler moduleAssembler)
        {
            _moduleRepo = moduleRepo;
            _moduleAssembler = moduleAssembler;
        }

        public void delete(long module_id)
        {
            try
            {
                using (var tx = _moduleRepo.beginTransaction())
                {
                    var module = _moduleRepo.getById(module_id) ?? throw new ItemNotFoundException($"Module with the id {module_id} doesnot exist.");

                    if (module.hasMenus())
                    {
                        throw new ItemNotFoundException($"Menus are assigned to specified module.");
                    }

                    _moduleRepo.delete(module);
                    _moduleRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void save(ModuleDto module_dto)
        {
            try
            {
                using (var tx = _moduleRepo.beginTransaction())
                {
                    bool isNameAllowed = isModuleNameAllowed(module_dto);
                    if (!isNameAllowed)
                    {
                        throw new DuplicateItemException("Module with same name already exists.");
                    }

                    var module = new Module();
                    _moduleAssembler.copy(module, module_dto);

                    _moduleRepo.insert(module);
                    _moduleRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void update(ModuleDto module_dto)
        {
            try
            {
                using (var tx = _moduleRepo.beginTransaction())
                {
                    var module = _moduleRepo.getById(module_dto.module_id) ?? throw new ItemNotFoundException($"Module with the id {module_dto.module_id} doesnot exist.");

                    bool isNameAllowed = isModuleNameAllowed(module_dto);

                    if (!isNameAllowed)
                    {
                        throw new DuplicateItemException("Module with same name already exists.");
                    }

                    _moduleAssembler.copy(module, module_dto);
                    _moduleRepo.update(module);
                    _moduleRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool isModuleNameAllowed(ModuleDto module_dto)
        {
            var moduleWithSameName = _moduleRepo.getByName(module_dto.module_name);
            return moduleWithSameName == null || moduleWithSameName.module_id == module_dto.module_id;
        }
    }
}
