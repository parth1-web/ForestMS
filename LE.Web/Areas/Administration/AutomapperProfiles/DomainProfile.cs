using AutoMapper;
using LE.Infrastructure.Dto;
using LE.Entities.User;
using LE.Web.Areas.Administration.Models;
using LE.Web.Areas.Administration.ViewModels;

namespace LE.Web.Areas.Administration.AutomapperProfiles
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<Role, RoleDetailModel>();
            CreateMap<RoleModel, Role>();
            CreateMap<Role, RoleModel>();
            CreateMap<LE.Entities.User.User, UserDetailModel>();
            CreateMap<UserModel, UserDto>();
            CreateMap<LE.Entities.User.User, UserModel>();
        }
    }
}
