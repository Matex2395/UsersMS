using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginMS.Custom;
using LoginMS.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using LoginMS.Data;
using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LoginMS.Interfaces;

namespace LoginMS.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IDistributedCache _cache;
        private readonly Utils _utils;

        public AccessController(
            AppDbContext appDbContext,
            IDistributedCache cache,
            Utils utils)
        {
            _appDbContext = appDbContext;
            _cache = cache;
            _utils = utils;
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            // Search for User
            var foundUser = await _appDbContext.TfaUsers
                                    .Where(u =>
                                        u.UserEmail == model.vls_email &&
                                        u.Contrasenia == _utils.encryptSHA256(model.vls_password)
                                    ).FirstOrDefaultAsync();

            if (foundUser == null)
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "" });
            }

            // Generate JWT
            var mainRole = await _appDbContext.TfaRols.Where(r => r.RolId == foundUser.RolId).FirstOrDefaultAsync();
            var extraRole = await _appDbContext.TfaRols.Where(r => r.RolId == foundUser.RolIdaddional).FirstOrDefaultAsync();
            var token = _utils.generateJWT(foundUser, mainRole?.RolName, extraRole?.RolName);

            // Store JWT in a Cookie
            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Use "true" in Production
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(24)
            });
            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token });
        }

        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Cookies["AuthToken"];
            if (token == null)
                return StatusCode(StatusCodes.Status401Unauthorized);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim != null)
            {
                var cacheKey = $"active_session_{userIdClaim.Value}";
                await _cache.RemoveAsync(cacheKey);
            }

            Response.Cookies.Delete("AuthToken");
            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
        }
    }
}
