using AutoMapper;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Web.Areas.Inventory.Models;
using LE.Web.Areas.Inventory.ViewModels;

namespace LE.Web.Areas.Inventory.AutomapperProfiles
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<CategoryPurposeModel, StockCategoryPurposeDto>().ReverseMap();
            CreateMap<CategoryPurposeModel, StockCategoryPurpose>().ReverseMap();
            CreateMap<WoodTypeModel, WoodTypeDto>().ReverseMap();
            CreateMap<WoodTypeModel, WoodType>().ReverseMap();
            CreateMap<StockUnitModel, StockUnitDto>().ReverseMap();
            CreateMap<StockUnitModel, StockUnit>().ReverseMap();
            CreateMap<WoodDetailsModel, WoodDetailsDto>().ReverseMap();
            CreateMap<WoodDetails, WoodsItemDetail>()
                .ForMember(dest => dest.wood_type, opt => opt.MapFrom(src => src.wood_type))
                .ForMember(dest => dest.piling, opt => opt.MapFrom(src => src.piling))
                .ForMember(dest => dest.category_purpose, opt => opt.MapFrom(src => src.category_purpose))
                .ReverseMap();
            CreateMap<Piling, Pilings>().ReverseMap();
            CreateMap<Piling, PilingDto>().ReverseMap();
            CreateMap<WoodDetails, WoodDetailsIndexViewModel>().ReverseMap();
        }
    }
}
