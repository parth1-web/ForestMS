using LE.Common.Exceptions;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Inventory.Service.Assemblers.Interface;
using LE.Inventory.Service.Services.Interface;
using System;
using System.Collections.Generic;

namespace LE.Inventory.Service.Services.Implementations
{
    public class DamagedWoodDetailServiceImpl : DamagedWoodDetailService
    {
        private readonly DamagedWoodDetailRepository _damagedWoodDetailRepository;
        private readonly DamagedWoodDetailAssembler _damagedWoodDetailAssembler;

        public DamagedWoodDetailServiceImpl(DamagedWoodDetailRepository damagedWoodDetailRepository, DamagedWoodDetailAssembler damagedWoodDetailAssembler)
        {
            _damagedWoodDetailRepository = damagedWoodDetailRepository;
            _damagedWoodDetailAssembler = damagedWoodDetailAssembler;
        }
        public void delete(long damaged_wood_detail_id)
        {
            try
            {
                var detail = _damagedWoodDetailRepository.getById(damaged_wood_detail_id);
                if (detail == null)
                {
                    throw new ItemNotFoundException("Damaged Wood Detail not found.");
                }
                _damagedWoodDetailRepository.delete(detail);
                _damagedWoodDetailRepository.saveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void save(List<DamagedWoodDetailDto> damagedWoodDetailDtos)
        {
            try
            {
                foreach (var detail in damagedWoodDetailDtos)
                {
                    DamagedWoodDetail damagedWoodDetail = new DamagedWoodDetail();
                    _damagedWoodDetailAssembler.copy(damagedWoodDetail, detail);
                    _damagedWoodDetailRepository.insert(damagedWoodDetail);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void update(List<DamagedWoodDetailDto> damagedWoodDetailDto)
        {
            try
            {
                foreach (var detail in damagedWoodDetailDto)
                {
                    var damagedData = _damagedWoodDetailRepository.getById(detail.damaged_wood_details_id);
                    if (damagedData == null)
                        throw new ItemNotFoundException("Damaged Wood Detail not found.");
                  //  DamagedWoodDetail damagedWoodDetail = new DamagedWoodDetail();
                    _damagedWoodDetailAssembler.copy(damagedData, detail);
                    _damagedWoodDetailRepository.update(damagedData);
                }
                _damagedWoodDetailRepository.saveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
