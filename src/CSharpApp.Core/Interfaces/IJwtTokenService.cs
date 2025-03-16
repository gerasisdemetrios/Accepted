
namespace CSharpApp.Core.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> GetTokenAsync();
        Task<string> RefreshTokenAsync();
    }
}