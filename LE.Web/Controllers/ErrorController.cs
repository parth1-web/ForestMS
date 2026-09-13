using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LE.Web.Controllers
{
    [Route("Error")]
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        // Accepts any HTTP method: StatusCodePagesWithReExecute re-executes failed
        // requests (including POSTs) against this route, and a method-restricted
        // handler would turn e.g. an antiforgery 400 into a confusing 405.
        [AcceptVerbs("GET", "POST", "PUT", "DELETE", "PATCH")]
        [Route("/error/{statusCode}")]
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
            else if (statusCode == 403)
            {
                // P1/S3: module permission denials (and cookie auth AccessDeniedPath)
                // land here.
                return View("Forbidden");
            }
            return View("Error");
        }
    }
}
