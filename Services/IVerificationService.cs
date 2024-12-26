namespace LoginMS.Services
{
    public interface IVerificationService
    {
        string GenerateCode();
        Task StoreVerificationCodeAsync(string email, string code);
        Task<bool> ValidateCodeAsync(string email, string code);
    }
}
