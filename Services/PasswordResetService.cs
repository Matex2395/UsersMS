
using LoginMS.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace LoginMS.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IDistributedCache _cache;
        private const int CODE_LENGTH = 4;
        private const int CODE_EXPIRATION_MINUTES = 5;

        public PasswordResetService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public string GenerateCode()
        {
            Random random = new Random();
            return random.Next(0, 9999).ToString("D4");
        }

        public async Task StoreVerificationCodeAsync(string email, string code)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CODE_EXPIRATION_MINUTES)
            };

            await _cache.SetStringAsync($"pwd_reset_{email}", code, options);
        }

        public async Task<bool> ValidateVerificationCodeAsync(string email, string code)
        {
            // Retrieve and validate the stored code
            var storedCode = await _cache.GetStringAsync($"pwd_reset_{email}");
            if (storedCode == null) return false;

            // If the code is valid, we eliminate it immediately for avoiding reusage
            if (storedCode == code)
            {
                await _cache.RemoveAsync($"pwd_reset_{email}");
                return true;
            }

            return false;
        }
    }
}
