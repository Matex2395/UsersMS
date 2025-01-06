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
        private readonly AppDbContext _appDbContext;
        private readonly Utils _utils;
        public UsersController(AppDbContext appDbContext, Utils utils)
        {
            _appDbContext = appDbContext;
            _utils = utils;
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var list = await _appDbContext.TfaUsers.ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = list });
        }

        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> CreateUser(UserDTO user)
        {
            // Search for Role
            var userRole = await _appDbContext.TfaRols.Where(u => u.RolName == user.vls_role).FirstOrDefaultAsync();
            if (userRole == null)
            {
                return NotFound(new { isSuccess = false, message = "Rol no encontrado." });
            }

            // Search for Extra Role
            var userExtraRole = await _appDbContext.TfaRols.Where(u => u.RolName == user.vls_extrarole).FirstOrDefaultAsync();

            // Create User
            var userModel = new TfaUser
            {
                UserName = user.vls_name,
                UserLastName = user.vls_lastname,
                UserEmail = user.vls_email,
                Contrasenia = _utils.encryptSHA256(user.vls_password),
                RolId = userRole.RolId,
                RolIdaddional = userExtraRole?.RolId
            };
            await _appDbContext.TfaUsers.AddAsync(userModel);
            await _appDbContext.SaveChangesAsync();

            if (userModel.UsersId != 0)
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
            // Search for User
            var existingUser = await _appDbContext.TfaUsers.FindAsync(id);
            if (existingUser == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = "Usuario No Encontrado" });
            }

            // Search for Role
            var userRole = await _appDbContext.TfaRols.Where(u => u.RolName == user.vls_role).FirstOrDefaultAsync();
            if (userRole == null)
            {
                return NotFound(new { isSuccess = false, message = "Rol no encontrado." });
            }

            // Search for Extra Role
            var userExtraRole = await _appDbContext.TfaRols.Where(u => u.RolName == user.vls_extrarole).FirstOrDefaultAsync();

            existingUser.UserName = user.vls_name;
            existingUser.UserLastName = user.vls_lastname;
            existingUser.UserEmail = user.vls_email;
            existingUser.RolId = userRole.RolId;
            existingUser.RolIdaddional = userExtraRole?.RolId;
            if (!string.IsNullOrEmpty(user.vls_password))
            {
                existingUser.Contrasenia = _utils.encryptSHA256(user.vls_password);
            }

            _appDbContext.TfaUsers.Update(existingUser);
            await _appDbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
        }

        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var existingUser = await _appDbContext.TfaUsers.FindAsync(id);
            if (existingUser == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = "Usuario No Encontrado" });
            }

            _appDbContext.TfaUsers.Remove(existingUser);
            await _appDbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
        }

        [HttpPost]
        [Route("CreateDefaultUsers")]
        public async Task<IActionResult> CreateDefaultUsers()
        {
            // Search for Role
            var adminUserRole = await _appDbContext.TfaRols.Where(u => u.RolName == "Administrador").FirstOrDefaultAsync();
            if (adminUserRole == null)
            {
                return NotFound(new { isSuccess = false, message = "Rol no encontrado." });
            }

            // Create Users
            var adminUserModel = new TfaUser
            {
                UserName = "Claire",
                UserLastName = "Redfield",
                UserEmail = "claire.redfield@umbrella.com",
                Contrasenia = _utils.encryptSHA256("claire123"),
                RolId = adminUserRole.RolId,
                RolIdaddional = null
            };
            await _appDbContext.TfaUsers.AddAsync(adminUserModel);
            await _appDbContext.SaveChangesAsync();


            var leaderUserRole = await _appDbContext.TfaRols.Where(u => u.RolName == "Lider").FirstOrDefaultAsync();
            if (leaderUserRole == null)
            {
                return NotFound(new { isSuccess = false, message = "Rol no encontrado." });
            }

            var leaderUserModel = new TfaUser
            {
                UserName = "Hermione",
                UserLastName = "Granger",
                UserEmail = "hermione.granger@hogwarts.com",
                Contrasenia = _utils.encryptSHA256("hermione123"),
                RolId = leaderUserRole.RolId,
                RolIdaddional = null
            };
            await _appDbContext.TfaUsers.AddAsync(leaderUserModel);
            await _appDbContext.SaveChangesAsync();


            var colaboratorUserRole = await _appDbContext.TfaRols.Where(u => u.RolName == "Colaborador").FirstOrDefaultAsync();
            if (colaboratorUserRole == null)
            {
                return NotFound(new { isSuccess = false, message = "Rol no encontrado." });
            }

            var colaboratorUserModel = new TfaUser
            {
                UserName = "Lara",
                UserLastName = "Croft",
                UserEmail = "lara.croft@tombraider.com",
                Contrasenia = _utils.encryptSHA256("lara123"),
                RolId = colaboratorUserRole.RolId,
                RolIdaddional = null
            };
            await _appDbContext.TfaUsers.AddAsync(colaboratorUserModel);
            await _appDbContext.SaveChangesAsync();

            var multipleRoleUserModel = new TfaUser
            {
                UserName = "Barry",
                UserLastName = "Burton",
                UserEmail = "barry.burton@rpd.com",
                Contrasenia = _utils.encryptSHA256("barry123"),
                RolId = adminUserRole.RolId,
                RolIdaddional = leaderUserModel.RolId
            };
            await _appDbContext.TfaUsers.AddAsync(multipleRoleUserModel);
            await _appDbContext.SaveChangesAsync();

            if (adminUserModel.UsersId != 0 || leaderUserModel.UsersId != 0 || colaboratorUserModel.UsersId != 0)
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false });
            }
        }
    }
}
