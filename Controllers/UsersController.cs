using LoginMS.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoginMS.Controllers
{
    [Route("api/[controller]")]
    //Define Authentication type. In this case, it's not necessary.
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MSDbContext _msDbContext;
        public UsersController(MSDbContext mSDbContext)
        {
            _msDbContext = mSDbContext;
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var list = await _msDbContext.Users.ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = list });
        }
    }
}
