using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginMS.Custom;
using LoginMS.Models;
using LoginMS.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using LoginMS.Data;

namespace LoginMS.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly MSDbContext _msDbContext;
        private readonly Utils _utils;
        public AccessController(MSDbContext msDbContext, Utils utils)
        {
            _msDbContext = msDbContext;
            _utils = utils;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            // Search for User
            var foundUser = await _msDbContext.Users
                                    .Where(u =>
                                        u.vls_email == model.vls_email &&
                                        u.vls_password == _utils.encryptSHA256(model.vls_password)
                                    ).FirstOrDefaultAsync();

            if (foundUser == null)
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "" });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token = _utils.generateJWT(foundUser) });
            }
        }
    }
}
