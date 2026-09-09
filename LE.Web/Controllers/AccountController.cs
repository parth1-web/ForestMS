using LE.Common.Enums;
using LE.Entities.User;
using LE.Service.Repository.Interface;
using LE.Service.Services.Interface;
using LE.Web.Helpers;
using LE.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    public class AccountController : BaseController
    {
        private readonly userNS.AuthenticationService _authenticationService;
        private LoginSessionService _loginSessionService;
        private OrganizationSetupRepository _orgSetupRepo;
        private UserRepository _userRepo;

        public AccountController(userNS.AuthenticationService authenticationService, LoginSessionService loginSessionService, UserRepository userRepo, OrganizationSetupRepository orgSetupRepo) : base()
        {
            _authenticationService = authenticationService;
            _loginSessionService = loginSessionService;
            _orgSetupRepo = orgSetupRepo;
            _userRepo = userRepo;
        }

        [Route("login")]
        public IActionResult login()
        {
            ViewBag.organizationName = _orgSetupRepo.getByKey(OrganizationSetup.Organization_Name.ToString())?.value;
            return View();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> login(LoginModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var authenticationDetail = _authenticationService.validateUser(model.username, model.password);

                    if (authenticationDetail == null)
                    {
                        throw new Exception("Username and password didnot match.");
                    }

                    var claims = new List<Claim>()
                    {
              new Claim(ClaimTypes.NameIdentifier,authenticationDetail.authentication_id.ToString())
                   };

                    var userIdentity = new ClaimsIdentity(claims, "local");

                    ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                    AuthenticationProperties prop = new AuthenticationProperties();
                    prop.ExpiresUtc = DateTime.UtcNow.AddDays(30);
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
                AlertHelper.setMessage(this, ex.Message, messageType.error);
            }
            return View(model);
        }

        [HttpPost]
        [Route("jwtlogin")]
        [IgnoreAntiforgeryToken]
        public IActionResult jwtLogin([FromBody] LoginModel model)
        {
            try
            {
                IActionResult response = Unauthorized();
                var authenticationDetail = _authenticationService.validateUser(model.username, model.password);


                if (authenticationDetail == null)
                {
                    throw new Exception("Username and password didnot match.");
                }

                var tokenString = GenerateToken(authenticationDetail);

                //record this token as valid

                var responseData = new
                {
                    user_id = authenticationDetail.authentication_id,
                    token = tokenString,
                    user = _userRepo.getById(authenticationDetail.type_id).full_name,
                    org_name = _orgSetupRepo.getByKey(OrganizationSetup.Organization_Name.ToString()).value,
                    address = _orgSetupRepo.getByKey(OrganizationSetup.Address.ToString()).value
                };
                return Content(JsonWrapper.buildSuccessJson(responseData), "application/json");
            }
            catch (Exception ex)
            {
                return Content(JsonWrapper.buildErrorJson(ex.Message), "application/json");
            }

        }

        private string GenerateToken(Authentication userInfo)
        {
            var claims = new Claim[] {
                        new Claim(JwtRegisteredClaimNames.Sub, userInfo.authentication_id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                        new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
                     };
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("thisisasecreteforauth"));
            SigningCredentials signingCredential = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            JwtHeader jwtHeader = new JwtHeader(signingCredential);
            JwtPayload jwtPayload = new JwtPayload(claims);
            JwtSecurityToken token = new JwtSecurityToken(jwtHeader, jwtPayload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Route("logout")]
        public async Task<IActionResult> logout()
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
    }
}