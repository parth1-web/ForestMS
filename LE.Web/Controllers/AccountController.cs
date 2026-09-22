using LE.Common.Enums;
using LE.Entities.User;
using LE.Service.Repository.Interface;
using LE.Service.Services.Interface;
using LE.Web.Helpers;
using LE.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LE.Web.Controllers
{
    using userNS = LE.Service.Services.Interface;
    [Route("account")]
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly userNS.AuthenticationService _authenticationService;
        private readonly LoginSessionService _loginSessionService;
        private readonly OrganizationSetupRepository _orgSetupRepo;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly LoginAttemptTracker _loginAttemptTracker;

        public AccountController(userNS.AuthenticationService authenticationService, LoginSessionService loginSessionService, UserRepository userRepo, OrganizationSetupRepository orgSetupRepo, Microsoft.Extensions.Configuration.IConfiguration configuration, LoginAttemptTracker loginAttemptTracker) : base()
        {
            _authenticationService = authenticationService;
            _loginSessionService = loginSessionService;
            _orgSetupRepo = orgSetupRepo;
            _userRepo = userRepo;
            _configuration = configuration;
            _loginAttemptTracker = loginAttemptTracker;
        }

        [Route("login")]
        public IActionResult Login()
        {
            ViewBag.organizationName = _orgSetupRepo.getByKey(OrganizationSetup.Organization_Name.ToString())?.value;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_loginAttemptTracker.IsLockedOut(model.username, HttpContext.Connection.RemoteIpAddress?.ToString()))
                    {
                        throw new Exception("Account is temporarily locked due to too many failed attempts. Please try again later.");
                    }

                    var authenticationDetail = _authenticationService.validateUser(model.username, model.password);

                    if (authenticationDetail == null)
                    {
                        _loginAttemptTracker.RecordFailure(model.username, HttpContext.Connection.RemoteIpAddress?.ToString());
                        throw new Exception("Username and password didnot match.");
                    }

                    _loginAttemptTracker.RecordSuccess(model.username, HttpContext.Connection.RemoteIpAddress?.ToString());

                    var claims = new List<Claim>()
                    {
              new Claim(ClaimTypes.NameIdentifier,authenticationDetail.authentication_id.ToString())
                   };

                    var userIdentity = new ClaimsIdentity(claims, "local");

                    ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                    AuthenticationProperties prop = new AuthenticationProperties();
                    int cookieExpirationHours = 8;
                    int.TryParse(_configuration["Security:CookieExpirationHours"], out cookieExpirationHours);
                    if (cookieExpirationHours <= 0) { cookieExpirationHours = 8; }
                    prop.ExpiresUtc = DateTime.UtcNow.AddHours(cookieExpirationHours);
                    prop.IsPersistent = model.remember_me;
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, prop);

                    var sessionDto = new LE.Infrastructure.Dto.LoginSessionDto()
                    {
                        authentication_id = authenticationDetail.authentication_id,
                        type = SessionType.login
                    };
                    _loginSessionService.save(sessionDto);
                    return Redirect("/home");
                }
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
            }
            return View(model);
        }

        [HttpPost]
        [Route("jwtlogin")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public IActionResult JwtLogin()
        {
            try
            {
                string rawBody = "";
                using (var reader = new System.IO.StreamReader(HttpContext.Request.Body))
                {
                    rawBody = reader.ReadToEndAsync().Result;
                }
                Console.WriteLine($"Raw request body: {rawBody}");

                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginModel>(rawBody);
                Console.WriteLine($"Deserialized model: username={model?.username}, password={model?.password}");

                if (model == null || string.IsNullOrWhiteSpace(model.username) || string.IsNullOrWhiteSpace(model.password))
                {
                    return Content(JsonWrapper.buildErrorJson("Username and password are required."), "application/json");
                }

                string remoteIp = HttpContext.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
                if (_loginAttemptTracker.IsLockedOut(model.username, remoteIp))
                {
                    return Content(JsonWrapper.buildErrorJson("Account is temporarily locked due to too many failed attempts. Please try again later."), "application/json");
                }

                var authenticationDetail = _authenticationService.validateUser(model.username, model.password);
                if (authenticationDetail == null)
                {
                    _loginAttemptTracker.RecordFailure(model.username, remoteIp);
                    throw new Exception("Username and password didnot match.");
                }

                _loginAttemptTracker.RecordSuccess(model.username, remoteIp);

                var tokenString = GenerateToken(authenticationDetail);

                var user = _userRepo.getById(authenticationDetail.type_id);
                if (user == null)
                {
                    return Content(JsonWrapper.buildErrorJson("User account not found."), "application/json");
                }

                var orgName = _orgSetupRepo.getByKey(OrganizationSetup.Organization_Name.ToString());
                var orgAddress = _orgSetupRepo.getByKey(OrganizationSetup.Address.ToString());

                var responseData = new
                {
                    user_id = authenticationDetail.authentication_id,
                    token = tokenString,
                    user = user.full_name,
                    org_name = orgName?.value ?? string.Empty,
                    address = orgAddress?.value ?? string.Empty
                };
                return Content(JsonWrapper.buildSuccessJson(responseData), "application/json");
            }
            catch (Exception ex)
            {
                return Content(ExceptionMessageHelper.buildErrorJson(ex), "application/json");
            }
        }

        private string GenerateToken(Authentication userInfo)
        {
            var claims = new Claim[] {
                        new Claim(JwtRegisteredClaimNames.Sub, userInfo.authentication_id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                        new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
                     };
            string issuer = _configuration["Jwt:Issuer"];
            string audience = _configuration["Jwt:Audience"];
            string signingKey = _configuration["Jwt:Key"];
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            SigningCredentials signingCredential = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            JwtHeader jwtHeader = new JwtHeader(signingCredential);
            JwtPayload jwtPayload = new JwtPayload(issuer, audience, claims, DateTime.Now, DateTime.Now.AddDays(1));
            JwtSecurityToken token = new JwtSecurityToken(jwtHeader, jwtPayload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Route("logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var authenticationId = getLoggedInAuthenticationId();
            await HttpContext.SignOutAsync();

            var sessionDto = new LE.Infrastructure.Dto.LoginSessionDto()
            {
                authentication_id = authenticationId,
                type = SessionType.logout
            };
            _loginSessionService.save(sessionDto);

            return Redirect("/account/login");
        }

        [Route("logout")]
        [HttpGet]
        public async Task<IActionResult> LogoutGet()
        {
            // Plain sign-out for legacy GET links/bookmarks. Logout is a state change,
            // so browsers should use the POST form in the header menu; this GET variant
            // does not accept an antiforgery token but still ends the session and
            // redirects, preserving the original navigation behavior.
            await HttpContext.SignOutAsync();
            return Redirect("/account/login");
        }
    }
}