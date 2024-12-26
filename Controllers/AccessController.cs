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
        private readonly MSDbContext _msDbContext;
        private readonly Utils _utils;
        private readonly IEmailSender _emailSender;
        private readonly IVerificationService _verificationService;

        public AccessController(
            MSDbContext msDbContext, 
            Utils utils,
            IEmailSender emailSender,
            IVerificationService verificationService)
        {
            _msDbContext = msDbContext;
            _utils = utils;
            _emailSender = emailSender;
            _verificationService = verificationService;
        }


        [HttpPost]
        [Route("IniciateLogin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IniciateLogin(LoginDTO model)
        {
            // Search for User
            var user = await _msDbContext.Users
            .FirstOrDefaultAsync(u =>
                u.vls_email == model.vls_email &&
                u.vls_password == _utils.encryptSHA256(model.vls_password));

            if (user == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new { message = "Credenciales inválidas" });
            }

            // Generate and store verification code
            var verificationCode = _verificationService.GenerateCode();
            await _verificationService.StoreVerificationCodeAsync(model.vls_email, verificationCode);

            // Send code via email
            await _emailSender.SendVerificationCodeAsync(model.vls_email, verificationCode);

            return Ok(new { message = "Código de verificación enviado" });
        }

        [HttpPost]
        [Route("VerifyCode")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerificationDTO model)
        {
            // Validate the code
            bool isValid = await _verificationService.ValidateCodeAsync(model.vls_email, model.vls_code);

            if (!isValid)
            {
                return BadRequest(new { message = "Código inválido o expirado" });
            }

            // Search for User and Generate JWT
            var user = await _msDbContext.Users
                .FirstOrDefaultAsync(u => u.vls_email == model.vls_email);

            if (user == null)
            {
                return BadRequest(new { message = "Usuario no encontrado" });
            }

            return Ok(new
            {
                isSuccess = true,
                token = _utils.generateJWT(user)
            });
        }
    }
}
