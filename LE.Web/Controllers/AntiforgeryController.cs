using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LE.Web.Controllers
{
    // P1 item 11: mutating actions are now POST + [ValidateAntiForgeryToken].
    // post-link.js calls this endpoint to obtain a token for pages that do not
    // render a form with @Html.AntiForgeryToken() (e.g. report list screens).
    [Authorize]
    [Route("Antiforgery")]
    public class AntiforgeryController : Controller
    {
        private readonly IAntiforgery _antiforgery;

        public AntiforgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        [HttpGet]
        [Route("Token")]
        public IActionResult Token()
        {
            // Standard Microsoft-documented pattern for exposing a token to
            // JavaScript: GetAndStoreTokens persists the pair and (re)issues the
            // companion antiforgery cookie on this response. The client must keep
            // that cookie alongside the returned request token.
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            return Json(new { token = tokens.RequestToken });
        }
    }
}
