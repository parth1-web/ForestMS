using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LE.Web.Controllers
{
    [Route("Error")]
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        [HttpGet("/error/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            if (statusCode == 404)
            {
                return View("NotFound");
            }
            else if (statusCode == 500)
            {
                return View("InternalError");
            }
            return View("Error");
        }
    }
}