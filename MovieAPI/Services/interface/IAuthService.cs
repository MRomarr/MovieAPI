
namespace MovieAPI.Services.@interface
{
    public interface IAuthService
    {
        Task<AuthDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthDto> LoginAsync(LoginDto loginDto);
        Task<bool> RevokeTokenAsync(string refreshToken);
        Task<string> AddRoleAsync(AddRoleDto model);
        Task<AuthDto> RefreshTokenAsync(string refreshToken);
    }
}