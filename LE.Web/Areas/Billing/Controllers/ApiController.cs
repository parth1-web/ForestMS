using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LE.Inventory.Infrastructure.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LE.Web.Areas.Billing.Controllers
{
    [Area("billing")]
    [Route("billing/api")]
    [AllowAnonymous]
    public class ApiController : Controller
    {
        private readonly WoodDetailsRepository _woodDetailsRepo;
        private readonly WoodTypeRepository _woodTypeRepo;
        private readonly StockItemRepository _stockItemRepo;

        public ApiController(WoodDetailsRepository woodDetailsRepo, WoodTypeRepository woodTypeRepo, StockItemRepository stockItemRepo)
        {
            _woodDetailsRepo = woodDetailsRepo;
            _woodTypeRepo = woodTypeRepo;
            _stockItemRepo = stockItemRepo;
        }

        [HttpGet]
        [Route("wood-detail/{id}")]
        public JsonResult woodDetail(long id)
        {
            var detail = _woodDetailsRepo.getQueryable().Where(a => a.wood_details_id == id).FirstOrDefault();
            if (detail == null)
                return Json(null);

            var holeTotal = detail.DamagedWoodDetails?.Sum(a => a.total_damaged_size) ?? 0;
            return Json(new
            {
                detail.wood_type_id,
                detail.circle_size,
                detail.length,
                net_total_size = detail.getNetTotal(),
                detail.fresh_total_size,
                hole_total_size = holeTotal,
                detail.stock_type_id,
                detail.balla_balli_category_id,
                damaged_items = detail.DamagedWoodDetails?.Select(p => new
                {
                    p.damaged_feet_size,
                    p.damaged_fifth_size,
                    p.damaged_first_size,
                    p.damaged_fourth_size,
                    p.damaged_second_size,
                    p.damaged_third_size,
                    p.dividor_value,
                    p.total_damaged_size,
                }).ToList(),
            });
        }


        [HttpGet]
        [Route("fire-wood-detail/{id}")]
        public JsonResult fireWoodDetail(long id)
        {
            var detail = _stockItemRepo.getQueryable().Where(a => a.stock_item_id == id);
            return Json(detail.Select(a => new
            {
                a.stock_item_id,
                a.default_sales_rate,
                a.wood_type.name,
            }));
        }

        [HttpGet]
        [Route("wood-type")]
        public JsonResult woodType()
        {
            var detail = _woodTypeRepo.getQueryable().Where(a => a.is_enabled == true);
            return Json(detail.Select(a => new
            {
                a.wood_type_id,
                a.name
            }));
        }
    }
}