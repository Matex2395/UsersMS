using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LoginMS.Models;

namespace LoginMS.Custom
{
    public class Utils
    {
        private readonly IConfiguration _configuration;
        public Utils(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Encrypting method
        public string encryptSHA256(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute the Hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convert byte array to string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public string generateJWT(User model)
        {
            // Create user info for the Token
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, model.vli_id.ToString()),
                new Claim(ClaimTypes.Name, model.vls_name),
                new Claim(ClaimTypes.Surname, model.vls_lastname),
                new Claim(ClaimTypes.Email, model.vls_email),
                new Claim(ClaimTypes.Role, model.vli_role.ToString()!),
                new Claim(ClaimTypes.Role, model.vli_extrarole.ToString()!)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // Create Token details
            var jwtConfig = new JwtSecurityToken(
                claims: userClaims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }
    }
}
