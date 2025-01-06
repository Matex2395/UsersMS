using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginMS.Custom;
using LoginMS.Models;
using LoginMS.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using LoginMS.Data;
using LoginMS.Services;

namespace LoginMS.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly Utils _utils;
        private readonly IEmailSender _emailService;
        private readonly IPasswordResetService _passwordResetService;

        public AccessController(
            AppDbContext appDbContext,
            Utils utils,
            IEmailSender emailSender,
            IPasswordResetService passwordResetService)
        {
            _appDbContext = appDbContext;
            _utils = utils;
            _emailService = emailSender;
            _passwordResetService = passwordResetService;
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
            else
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token = _utils.generateJWT(foundUser) });
            }
        }

        [HttpPost("RequestReset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetDTO request)
        {
            // Verify if the user exists or not
            var user = await _appDbContext.TfaUsers
                .FirstOrDefaultAsync(u => u.UserEmail == request.vls_email);

            if (user == null)
            {
                return Ok(new
                {
                    message = "Si el correo existe en nuestro sistema, recibirás un código de verificación."
                });
            }

            // Generate and store the code
            var verificationCode = _passwordResetService.GenerateCode();
            await _passwordResetService.StoreVerificationCodeAsync(request.vls_email, verificationCode);

            // Send the code via email
            await _emailService.SendPasswordResetCodeAsync(request.vls_email, verificationCode);

            return Ok(new
            {
                message = "Si el correo existe en nuestro sistema, recibirás un código de verificación."
            });
        }

        [HttpPost("Reset")]
        public async Task<IActionResult> ResetPassword([FromBody] VerifyAndResetPasswordDTO request)
        {
            // Verify the code
            bool isCodeValid = await _passwordResetService.ValidateVerificationCodeAsync(
                request.vls_email,
                request.vls_code
            );

            if (!isCodeValid)
            {
                return BadRequest(new
                {
                    message = "El código de verificación es inválido o ha expirado."
                });
            }

            // Search for User
            var user = await _appDbContext.TfaUsers
                .FirstOrDefaultAsync(u => u.UserEmail == request.vls_email);

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            // Update password
            user.Contrasenia = _utils.encryptSHA256(request.vls_newpassword);
            await _appDbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Contraseña actualizada exitosamente."
            });
        }
    }
}
