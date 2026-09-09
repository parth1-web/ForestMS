using LE.Billing.Infrastructure.Repository.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LE.Web.Areas.Counter.Controllers
{
    [Route("api/service-category")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class ServiceCategoryApiController : ControllerBase
    {
        private readonly ServiceCategoryRepository _serviceCategoryRepo;

        public ServiceCategoryApiController(ServiceCategoryRepository serviceCategoryRepo)
        {
            _serviceCategoryRepo = serviceCategoryRepo;
        }

        [HttpGet]
        [Route("")]
        [Route("index")]
        public IActionResult get()
        {
            var categories = _serviceCategoryRepo.getAll();
            var responseData = categories.Select(a => new
            {
                a.category_id,
                a.name,
                a.is_enabled
            }).ToList();
            return Ok(responseData);
        }
    }
}