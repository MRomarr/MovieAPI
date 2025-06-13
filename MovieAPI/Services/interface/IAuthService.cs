namespace MovieAPI.Services.@interface
{
    public interface IAuthService
    {
        Task<AuthModel> Register(RegisterAuth model);
        Task<AuthModel> Login(TokenRequestModel model);
        Task<string> AddRole(AddRoleModel model);
        Task<AuthModel> RefreshToken(string refreshToken);
        Task<bool> RevokeToken(string token);
    }
}
