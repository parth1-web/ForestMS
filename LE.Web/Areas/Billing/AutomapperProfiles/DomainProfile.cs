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

			CreateMap<ServiceDto, ServiceModel>().ReverseMap();

			CreateMap<MemberModel, MemberDto>().ReverseMap();

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
				.ForMember(md => md.ImageName, opt => opt.MapFrom(src => src.ImageName.FileName ?? string.Empty));

			CreateMap<Membership, MembershipIndexViewModel>().ReverseMap();
			CreateMap<MemberPunishment, MemberPunishmentDto>().ReverseMap();
		}
	}
}
