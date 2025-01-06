﻿using LoginMS.Custom;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdministradorOnly")]
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
    }
}
