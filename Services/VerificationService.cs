
using Microsoft.Extensions.Caching.Distributed;

namespace LoginMS.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly IDistributedCache _cache;
        private const int CODE_LENGTH = 4;
        private const int CODE_EXPIRATION_MINUTES = 5;

        public VerificationService(IDistributedCache cache)
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

            await _cache.SetStringAsync(email, code, options);
        }

        public async Task<bool> ValidateCodeAsync(string email, string code)
        {
            var storedCode = await _cache.GetStringAsync(email);
            return storedCode == code;
        }
    }
}
