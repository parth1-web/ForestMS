using LE.Common.Exceptions;
using LE.Entities.User;
using LE.Infrastructure.Dto;
using LE.Service.Assembler.Interface;
using LE.Service.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Service.Assembler.Implementations
{
    public class DynamicMenuAssemblerImpl : DynamicMenuAssembler
    {
        private ModuleRepository _moduleRepo;
        private DynamicMenuRepository _dynamicMenuRepo;
        public DynamicMenuAssemblerImpl(ModuleRepository moduleRepo,DynamicMenuRepository dynamicMenuRepo)
        {
            _moduleRepo = moduleRepo;
            _dynamicMenuRepo = dynamicMenuRepo;
        }
        public void copy(DynamicMenu dynamic_menu, DynamicMenuDto dynamic_menu_dto)
        {
            dynamic_menu.menu_name = dynamic_menu_dto.menu_name;
            dynamic_menu.module_id = dynamic_menu_dto.module_id;

            dynamic_menu.parent_menu_id = dynamic_menu_dto.parent_menu_id;

            dynamic_menu.icon = dynamic_menu_dto.icon;
            dynamic_menu.web_url = dynamic_menu_dto.web_url;
            dynamic_menu.api_url = dynamic_menu_dto.api_url;

            dynamic_menu.display_order = dynamic_menu_dto.display_order;

            dynamic_menu.module = _moduleRepo.getById(dynamic_menu_dto.module_id) ?? throw new ItemNotFoundException($"Module with id {dynamic_menu_dto.module_id} doesnot exist.");

            if (dynamic_menu_dto.parent_menu_id.HasValue && dynamic_menu_dto.parent_menu_id > 0)
            {
                dynamic_menu.parent_menu = _dynamicMenuRepo.getById((long)dynamic_menu_dto.parent_menu_id) ?? throw new ItemNotFoundException($"Dynamic menu with id {dynamic_menu_dto.parent_menu_id} doesnot exist.");
            }
        }
    }
}
