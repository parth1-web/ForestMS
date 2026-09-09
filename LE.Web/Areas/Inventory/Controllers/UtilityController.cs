using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LE.Web.Areas.Inventory.Controllers
{
    [Authorize]
    [Area("inventory")]
    [Route("inventory/utility")]
    public class UtilityController : Controller
    {
        private readonly WoodDetailsRepository _woodDetailsRepo;
        private readonly DamagedWoodDetailRepository _damagedWoodDetailRepo;

        public UtilityController(WoodDetailsRepository woodDetailsRepository,DamagedWoodDetailRepository damagedWoodDetailRepository)
        {
            _woodDetailsRepo = woodDetailsRepository;
            _damagedWoodDetailRepo = damagedWoodDetailRepository;
        }
        [Route("port")]
        [HttpGet]
        public IActionResult Port()
        {
            try
            {
                //var woodDetails = _woodDetailsRepo.getQueryable().Where(a=>a.hole_total_size>0).ToList();
                //foreach(var detail in woodDetails)
                //{
                //    DamagedWoodDetail d = new DamagedWoodDetail();
                //    d.damaged_first_size = detail.damaged_first_size;
                //    d.damaged_second_size = detail.damaged_second_size;
                //    d.damaged_feet_size = detail.damaged_third_size;
                //    d.dividor_value = 2;
                //    d.total_damaged_size = detail.hole_total_size;
                //    d.wood_details_id = detail.wood_details_id;

                //    _damagedWoodDetailRepo.insert(d);
               // }
                AlertHelper.setMessage(this, "Successfully ported data.", messageType.success);
                return Redirect("/home");
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return Redirect("/home");
            }
        }
    }
}
