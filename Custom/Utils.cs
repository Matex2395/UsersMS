using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LoginMS.Models;
using LoginMS.Data;
using Microsoft.Extensions.Caching.Distributed;

namespace LoginMS.Custom
{
    public class Utils
    {
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;
        private readonly AppDbContext _appDbContext;
        public Utils(IConfiguration configuration, IDistributedCache cache, AppDbContext appDbContext)
        {
            _configuration = configuration;
            _cache = cache;
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
            // Generate new Session ID
            var sessionId = Guid.NewGuid().ToString();
            Console.WriteLine($"Generated SessionId: {sessionId}");

            // Store Session ID in caché
            _cache.SetString($"active_session_{sessionId}", "active", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            var mainRole = _appDbContext.TfaRols.Where(r => r.RolId == model.RolId).FirstOrDefault();
            var extraRole = _appDbContext.TfaRols.Where(r => r.RolId == model.RolIdaddional).FirstOrDefault();

            // Create user info for the Token
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, model.UsersId.ToString()),
                new Claim(ClaimTypes.Name, model.UserName),
                new Claim(ClaimTypes.Surname, model.UserLastName),
                new Claim(ClaimTypes.Email, model.UserEmail),
                new Claim(ClaimTypes.Role, mainRole!.RolName),
                new Claim(ClaimTypes.Role, extraRole!.RolName),
                new Claim("SessionId", sessionId),
                new Claim("Timestamp", DateTime.UtcNow.ToString("o"))
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
