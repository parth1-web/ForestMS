using LE.Common.Exceptions;
using LE.Entities.User;
using LE.Infrastructure.Dto;
using LE.Service.Assembler.Interface;
using LE.Service.Repository.Interface;
using LE.Service.Services.Interface;
using System;
using System.Transactions;

namespace LE.Service.Services.Implementations
{
    public class DynamicMenuServiceImpl : DynamicMenuService
    {
        private readonly DynamicMenuRepository _dynamicMenuRepo;
        private readonly DynamicMenuAssembler _dynamicMenuAssembler;

        public DynamicMenuServiceImpl(DynamicMenuRepository dynamicMenuRepo, DynamicMenuAssembler dynamicMenuAssembler)
        {
            _dynamicMenuAssembler = dynamicMenuAssembler;
            _dynamicMenuRepo = dynamicMenuRepo;
        }

        public void delete(long menu_id)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var dynamicMenu = _dynamicMenuRepo.getById(menu_id) ?? throw new ItemNotFoundException($"Dynamic menu with id {menu_id} doesnot exist.");

                    if (dynamicMenu.hasSubMenus())
                    {
                        throw new ChildCollectionsPresentException("Sub menus are present in specified menu.");
                    }

                    _dynamicMenuRepo.delete(dynamicMenu);
                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DynamicMenu save(DynamicMenuDto dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    bool isMenuNameNotAllowed = isMenuNameDuplicateInSameModule(dto);

                    if (isMenuNameNotAllowed)
                    {
                        throw new DuplicateItemException("Menu with same name already exists in module.");
                    }

                    var dynamicMenu = new DynamicMenu();
                    _dynamicMenuAssembler.copy(dynamicMenu, dto);
                    _dynamicMenuRepo.insert(dynamicMenu);

                    tx.Complete();
                    return dynamicMenu;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void update(DynamicMenuDto dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var dynamicMenu = _dynamicMenuRepo.getById(dto.dynamic_menu_id) ?? throw new ItemNotFoundException($"Dynamic menu with id {dto.dynamic_menu_id} doesnot exist.");

                    bool isMenuNameNotAllowed = isMenuNameDuplicateInSameModule(dto);

                    if (isMenuNameNotAllowed)
                    {
                        throw new DuplicateItemException("Menu with same name already exists in module.");
                    }

                    _dynamicMenuAssembler.copy(dynamicMenu, dto);
                    _dynamicMenuRepo.update(dynamicMenu);
                    tx.Complete();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private bool isMenuNameDuplicateInSameModule(DynamicMenuDto dto)
        {
            var menuWithSameNameInModule = _dynamicMenuRepo.getByModuleAndMenuName(dto.module_id, dto.menu_name);

            if (menuWithSameNameInModule != null && menuWithSameNameInModule.dynamic_menu_id != dto.dynamic_menu_id)
            {
                return true;
            }
            return false;
        }
    }
}
