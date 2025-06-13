using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MovieAPI.Helpers;
using MovieAPI.Services.@interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MovieAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManger;
        private readonly JWT _jwt;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _roleManger = roleManager;
            _jwt = jwt.Value;
        }
        public async Task<AuthModel> Register(RegisterAuth model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Massage = "Email already exists" };
            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Massage = "Username already exists" };
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Username,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var error = string.Empty;
                foreach (var errorItem in result.Errors)
                {
                    error += $"{errorItem.Description} ";
                }
                return new AuthModel { Massage = error };
            }
            await _userManager.AddToRoleAsync(user, "User");
            var JWTSecurityToken = await CreateJwtToken(user);
            return new AuthModel
            {
                IsAuthenticated = true,
                Username = user.UserName,
                Email = user.Email,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(JWTSecurityToken),
                
            };

        }

        public async Task<AuthModel> Login(TokenRequestModel model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Massage = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            //authModel.Expiration = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();
            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.Expairation;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.Expairation;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }
            return authModel;
        }

        public async Task<string> AddRole(AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManger.RoleExistsAsync(model.RoleName))
                return "Invalid user ID or Role";

            if (await _userManager.IsInRoleAsync(user, model.RoleName))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            return result.Succeeded ? string.Empty : "Sonething went wrong";
        }

        public async Task<AuthModel> RefreshToken(string token)
        {
            var authModel = new AuthModel();

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                authModel.Massage = "Invalid token";
                return authModel;
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                authModel.Massage = "Inactive token";
                return authModel;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            authModel.Roles = roles.ToList();
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.Expairation;

            return authModel;

        }
        public async Task<bool> RevokeToken(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return true;
        }


        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomNumber),
                    Expairation = DateTime.UtcNow.AddDays(10)
                };
            }
        }

        
    }
}
