namespace LoginMS.Interfaces
{
    public interface IPasswordResetService
    {
        string GenerateCode();
        Task StoreVerificationCodeAsync(string email, string code);
        Task<bool> ValidateVerificationCodeAsync(string email, string code);
    }
}
