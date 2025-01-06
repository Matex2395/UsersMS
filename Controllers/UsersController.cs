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
            var userModel = new TfaUser
            {
                UserName = user.vls_name,
                UserLastName = user.vls_lastname,
                UserEmail = user.vls_email,
                Contrasenia = _utils.encryptSHA256(user.vls_password)
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
            var existingUser = await _appDbContext.TfaUsers.FindAsync(id);
            if (existingUser == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = "Usuario No Encontrado" });
            }

            existingUser.UserName = user.vls_name;
            existingUser.UserLastName = user.vls_lastname;
            existingUser.UserEmail = user.vls_email;
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
        [Route("CreateRoles")]
        public async Task<IActionResult> CreateRoles()
        {
            var AdminRolModel = new TfaRol
            {
                RolName = "Administrador",
                RolDescription = "Rol que tiene acceso a todas las funciones del sistema"
            };
            await _appDbContext.TfaRols.AddAsync(AdminRolModel);
            await _appDbContext.SaveChangesAsync();

            var LeaderRolModel = new TfaRol
            {
                RolName = "Lider",
                RolDescription = "Rol que puede completar tareas, otorgar diplomas a colaboradores y generar reportes"
            };
            await _appDbContext.TfaRols.AddAsync(LeaderRolModel);
            await _appDbContext.SaveChangesAsync();

            var ColaboratorRolModel = new TfaRol
            {
                RolName = "Colaborador",
                RolDescription = "Rol que puede completar tareas y recibir diplomas"
            };
            await _appDbContext.TfaRols.AddAsync(ColaboratorRolModel);
            await _appDbContext.SaveChangesAsync();

            if (AdminRolModel.RolId != 0 || LeaderRolModel.RolId != 0 || ColaboratorRolModel.RolId != 0)
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
