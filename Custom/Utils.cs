using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LoginMS.Models;
using LoginMS.Data;

namespace LoginMS.Custom
{
    public class Utils
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _appDbContext;
        public Utils(IConfiguration configuration, AppDbContext appDbContext)
        {
            _configuration = configuration;
            _appDbContext = appDbContext;
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

        public string generateJWT(TfaUser model)
        {
            // Search for Role
            var userRole = _appDbContext.TfaRols.Where(u => u.RolId == model.RolId).FirstOrDefault();

            // Search for Extra Role
            var userExtraRole = _appDbContext.TfaRols.Where(u => u.RolId == model.RolIdaddional).FirstOrDefault();

            // Create user info for the Token
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, model.UsersId.ToString()),
                new Claim(ClaimTypes.Name, model.UserName),
                new Claim(ClaimTypes.Surname, model.UserLastName),
                new Claim(ClaimTypes.Email, model.UserEmail)
            };

            // Add primary role claim
            if (userRole != null)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, userRole.RolName));
            }

            // Add extra role claim if it exists
            if (userExtraRole != null)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, userExtraRole.RolName));
            }

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
