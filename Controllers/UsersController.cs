﻿using LoginMS.Custom;
using LoginMS.Data;
using LoginMS.Interfaces;
using LoginMS.Models;
using LoginMS.Models.DTOs;
using LoginMS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoginMS.Controllers
{
    [Route("api/[controller]")]
    //Define Authentication type. In this case, it's not necessary.
    //[Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly Utils _utils;
        private readonly IFileUploadService _uploadService;
        public UsersController(AppDbContext appDbContext, Utils utils, IFileUploadService fileUploadService)
        {
            _appDbContext = appDbContext;
            _utils = utils;
            _uploadService = fileUploadService;
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

            // Upload image if it's provided
            if (user.vlf_image != null)
            {
                userModel.UrlImage = await _uploadService.UploadUserImageAsync(user.vlf_image);
            }

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
        public async Task<IActionResult> EditUser(int id, UpdateUserDTO user)
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

            // If a new password is provided, it's updated
            if (!string.IsNullOrEmpty(user.vls_password))
            {
                existingUser.Contrasenia = _utils.encryptSHA256(user.vls_password);
            }

            // If a new image is provided, the Image is uploaded and its URL property is updated
            if (user.vlf_image != null)
            {
                existingUser.UrlImage = await _uploadService.UploadUserImageAsync(user.vlf_image);
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

        [HttpGet]
        [Route("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            // Buscar usuario por ID
            var user = await _appDbContext.TfaUsers
                .Where(u => u.UsersId == id)
                .Select(u => new
                {
                    u.UsersId,
                    u.UserName,
                    u.UserLastName,
                    u.UserEmail,
                    u.UrlImage,
                    Role = _appDbContext.TfaRols.Where(r => r.RolId == u.RolId).Select(r => r.RolName).FirstOrDefault(),
                    ExtraRole = _appDbContext.TfaRols.Where(r => r.RolId == u.RolIdaddional).Select(r => r.RolName).FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = "Usuario no encontrado" });
            }

            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, user });
        }

        [HttpGet]
        [Route("GetUsersByRol/{role}")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            var foundRole = await _appDbContext.TfaRols.Where(r => r.RolName == role).FirstOrDefaultAsync();

            if (foundRole == null)
            {
                return NotFound(new { isSuccess = false, message = "Rol no encontrado." });
            }

            var usersByRole = await _appDbContext.TfaUsers
                                        .Where(u => u.RolId == foundRole.RolId)
                                            .Select(u => new SearchBarUsersDTO
                                            {
                                                vli_id = u.UsersId,
                                                vls_name = u.UserName,
                                                vls_lastname = u.UserLastName,
                                                vls_email = u.UserEmail
                                            })
                                            .ToListAsync();

            return usersByRole.Any() ? Ok(usersByRole) : NotFound("No users found with the specified role.");
        }
    }
}
