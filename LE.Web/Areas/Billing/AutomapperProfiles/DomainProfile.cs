using AutoMapper;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Web.Areas.Billing.Models;
using LE.Web.Areas.Billing.ViewModels;

namespace LE.Web.Areas.Billing.AutomapperProfiles
{
	public class DomainProfile : Profile
	{
		public DomainProfile()
		{
			CreateMap<ServiceCategoryModel, ServiceCategoryDto>().ReverseMap();

			CreateMap<ServiceCategory, ServiceCategoryDetail>().ReverseMap();

			CreateMap<FurnitureCategoryModel, FurnitureCategoryDto>().ReverseMap();

			CreateMap<FurnitureCategoryModel, FurnitureCategory>().ReverseMap();

			// Without this map every furniture add/edit throws
			// AutoMapperMappingException (shown as a generic error).
			CreateMap<FurnitureModel, FurnitureDto>().ReverseMap();

			CreateMap<ServiceDto, ServiceModel>().ReverseMap();

			// MemberModel.ImageName is an uploaded file (IFormFile) while
			// MemberDto.ImageName is the saved file name (string): the file
			// itself can never auto-map, and trying throws on every save.
			// Callers persist the file first and set the name explicitly.
			CreateMap<MemberModel, MemberDto>()
				.ForMember(d => d.ImageName, opt => opt.Ignore())
				.ReverseMap()
				.ForMember(m => m.ImageName, opt => opt.Ignore());

			CreateMap<MemberDetail, Member>().ReverseMap();

			CreateMap<Tole, ToleDetail>().ReverseMap();

			CreateMap<BillDetail, WoodBill>().ReverseMap();

			CreateMap<WoodDetailsDto, WoodDetails>().ReverseMap();

			CreateMap<FirewoodBillReportDetails, FirewoodSales>().ReverseMap();

			CreateMap<FurnitureBillReportDetails, FurnitureSales>().ReverseMap();

			CreateMap<FurnitureDetail, Furniture>().ReverseMap();

			CreateMap<WoodBill, WoodBillReportDetails>().ReverseMap();

			CreateMap<CounterSales, CounterBillDetail>().ReverseMap();

			CreateMap<WoodBillDetailDto, WoodDetails>().ReverseMap();

			CreateMap<CounterSalesDetail, CounterBillDetail>().ReverseMap();

			CreateMap<MembershipValidityDto, MembershipValidityModel>().ReverseMap();

			CreateMap<Membership, MembershipModel>().ReverseMap();

			CreateMap<MembershipValidity, MembershipValidityModel>().ReverseMap();

			CreateMap<MemberDetail, MemberModel>();

			CreateMap<MemberModel, MemberDetail>()
				.ForMember(md => md.ImageName, opt => opt.MapFrom(src => src.ImageName != null ? src.ImageName.FileName : string.Empty));

			CreateMap<Membership, MembershipIndexViewModel>().ReverseMap();
			CreateMap<MemberPunishment, MemberPunishmentDto>().ReverseMap();
		}
	}
}
