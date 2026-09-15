using LE.Common.Exceptions;
using LE.Inventory.Common.Enums;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Inventory.Service.Assemblers.Interface;
using LE.Inventory.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Inventory.Service.Services.Implementations
{
    public class WoodDetailsServiceImpl : WoodDetailsService
    {
        private readonly WoodDetailsRepository _woodDetailsRepo;
        private readonly WoodDetailsAssembler _woodDetailsAssembler;
        private readonly DamagedWoodDetailService _damagedWoodDetailService;

        public WoodDetailsServiceImpl(WoodDetailsRepository woodDetailsRepo, WoodDetailsAssembler woodDetailsAssembler, DamagedWoodDetailService damagedWoodDetailService)
        {
            _woodDetailsRepo = woodDetailsRepo;
            _woodDetailsAssembler = woodDetailsAssembler;
            _damagedWoodDetailService = damagedWoodDetailService;
        }

        public void delete(long wood_details_id)
        {
            using (var tx = _woodDetailsRepo.beginTransaction())
            {
                var woodDetails = _woodDetailsRepo.getById(wood_details_id);

                if (woodDetails == null)
                    throw new ItemNotFoundException($"The Wood with {wood_details_id} doesnot exist.");

                if (woodDetails.is_sold == true)
                {
                    throw new ItemUsedException("This wood is already sold.You cannot delete at a moment.");
                }

                _woodDetailsRepo.delete(woodDetails);
                _woodDetailsRepo.saveChanges();
                tx.Commit();
            }
        }



        public void save(WoodDetailsDto wood_details_dto)
        {
            using (var tx = _woodDetailsRepo.beginTransaction())
            {
                WoodDetails wood_details = new WoodDetails();

                var woodDetails = _woodDetailsRepo.getByGoliaNo(wood_details_dto.goliya_number, wood_details_dto.year);

                checkIfAlreadyExists(wood_details_dto, woodDetails);

                _woodDetailsAssembler.copy(wood_details, wood_details_dto);

                wood_details.setFreshTotalSize();
                _woodDetailsRepo.insert(wood_details);

                if (wood_details_dto.DamagedWoodDetailDtos.Count > 0)
                {
                    wood_details_dto.DamagedWoodDetailDtos.ForEach(a => a.wood_details_id = wood_details.wood_details_id);
                    _damagedWoodDetailService.save(wood_details_dto.DamagedWoodDetailDtos);
                }

                _woodDetailsRepo.saveChanges();
                tx.Commit();
            }
        }

        private void checkIfAlreadyExists(WoodDetailsDto wood_details_dto, List<WoodDetails> woodDetails)
        {
            if (wood_details_dto.stock_type_id == Convert.ToInt32(StockTypes.BallaBalli))
            {
                if (wood_details_dto.balla_balli_category_id == Convert.ToInt32(BallaBalliCategorys.KhabaKhutti))
                {
                    var details = woodDetails.Where(a => a.balla_balli_category_id == Convert.ToInt32(BallaBalliCategorys.KhabaKhutti) && a.goliya_number.CompareTo(wood_details_dto.goliya_number) == 0).ToList();
                    if (details.Count > 0)
                    {
                        throw new DuplicateItemException("Golia with this number in this category for this year already exist.");
                    }
                }
                if (wood_details_dto.balla_balli_category_id == Convert.ToInt32(BallaBalliCategorys.Size))
                {
                    // P1/B10 fix: was 'a.goliya_number.CompareTo(a.goliya_number)' which is
                    // always 0 — every existing Size-category log was treated as a duplicate.
                    var details = woodDetails.Where(a => a.balla_balli_category_id == Convert.ToInt32(BallaBalliCategorys.Size) && a.goliya_number.CompareTo(wood_details_dto.goliya_number) == 0).ToList();
                    if (details.Count > 0)
                    {
                        throw new DuplicateItemException("Golia with this number in this category for this year already exist.");
                    }
                }

            }
            if (wood_details_dto.stock_type_id == Convert.ToInt32(StockTypes.Lakadi))
            {
                var details = woodDetails.Where(a => a.stock_type_id == Convert.ToInt32(StockTypes.Lakadi) && a.goliya_number.CompareTo(wood_details_dto.goliya_number) == 0).ToList();
                if (details.Count > 0)
                {
                    throw new DuplicateItemException("Golia with this number in this category for this year already exist.");
                }
            }
        }

        public void update(WoodDetailsDto wood_details_dto)
        {
            using (var tx = _woodDetailsRepo.beginTransaction())
            {
                var woodDetails = _woodDetailsRepo.getById(wood_details_dto.wood_details_id);
                if (woodDetails == null)
                    throw new ItemNotFoundException($"The wood with id {wood_details_dto.wood_details_id} doesnot exist");

                if (woodDetails.is_sold == true)
                {
                    throw new ItemUsedException("This wood is already sold.You cannot update it.");
                }

                wood_details_dto.created_date = woodDetails.created_date;
                _woodDetailsAssembler.copy(woodDetails, wood_details_dto);
                woodDetails.setFreshTotalSize();
                _woodDetailsRepo.update(woodDetails);

                if (wood_details_dto.DamagedWoodDetailDtos.Count > 0)
                {
                    wood_details_dto.DamagedWoodDetailDtos.ForEach(a => a.wood_details_id = woodDetails.wood_details_id);
                    _damagedWoodDetailService.update(wood_details_dto.DamagedWoodDetailDtos);
                }

                _woodDetailsRepo.saveChanges();
                tx.Commit();
            }
        }
    }
}
