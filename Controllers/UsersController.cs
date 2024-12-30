using LoginMS.Custom;
using LoginMS.Data;
using LoginMS.Models;
using LoginMS.Models.DTOs;
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
        private readonly Utils _utils;
        public UsersController(MSDbContext mSDbContext, Utils utils)
        {
            _msDbContext = mSDbContext;
            _utils = utils;
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var list = await _msDbContext.Users.ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = list });
        }

        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> CreateUser(UserDTO user)
        {
            var userModel = new User
            {
                vls_name = user.vls_name,
                vls_lastname = user.vls_lastname,
                vls_email = user.vls_email,
                vls_password = _utils.encryptSHA256(user.vls_password)
            };
            await _msDbContext.Users.AddAsync(userModel);
            await _msDbContext.SaveChangesAsync();

            if (userModel.vli_id != 0)
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false });
            }
        }

        [HttpPut]
        [Route("EditUser")]
        public async Task<IActionResult> EditUser(int id, UserDTO user)
        {
            var existingUser = await _msDbContext.Users.FindAsync(id);
            if (existingUser == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = "User not found" });
            }

            existingUser.vls_name = user.vls_name;
            existingUser.vls_lastname = user.vls_lastname;
            existingUser.vls_email = user.vls_email;
            if (!string.IsNullOrEmpty(user.vls_password))
            {
                existingUser.vls_password = _utils.encryptSHA256(user.vls_password);
            }

            _msDbContext.Users.Update(existingUser);
            await _msDbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
        }

        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var existingUser = await _msDbContext.Users.FindAsync(id);
            if (existingUser == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = "User not found" });
            }

            _msDbContext.Users.Remove(existingUser);
            await _msDbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
        }
    }
}
