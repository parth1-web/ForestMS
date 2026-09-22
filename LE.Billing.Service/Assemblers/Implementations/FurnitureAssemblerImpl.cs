using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Common.Exceptions;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class FurnitureAssemblerImpl : FurnitureAssembler
    {
        private readonly FurnitureCategoryRepository _furnitureCategoryRepo;

        public FurnitureAssemblerImpl(FurnitureCategoryRepository furnitureCategoryRepo)
        {
            _furnitureCategoryRepo = furnitureCategoryRepo;
        }

        public void copy(Furniture furniture, FurnitureDto furniture_dto)
        {
            furniture.furniture_category_id = furniture_dto.furniture_category_id;
            furniture.name = furniture_dto.name;
            furniture.is_enabled = furniture_dto.is_enabled;
            furniture.furnitureCategory = _furnitureCategoryRepo.getById(furniture_dto.furniture_category_id) ?? throw new ItemNotFoundException($"Furniture category with the id {furniture_dto.furniture_category_id} does not exist.");
        }
    }
}
