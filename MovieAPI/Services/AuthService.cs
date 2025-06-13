

namespace MoviesAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWThelper _jwt;
        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JWThelper> Jwt, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = Jwt.Value;
        }

        public async Task<AuthDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if email already exists
            if (await _userManager.FindByEmailAsync(registerDto.Email) is not null)
                return new AuthDto { Message = "Email already exists." };

            // Check if username already exists
            if (await _userManager.FindByNameAsync(registerDto.UserName) is not null)
                return new AuthDto { Message = "UserName already exists." };

            // Create new user
            var user = new ApplicationUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
            };

            // Attempt to create user with password
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthDto { Message = errors };
            }

            // Assign default role
            await _userManager.AddToRoleAsync(user, "User");

            // Generate JWT token
            var token = await CreateJWTToken(user);

            return new AuthDto
            {
                Message = "Registration successful!",
                UserName = user.UserName,
                Email = user.Email,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                TokenExpiration = token.ValidTo
            };
        }
        public async Task<AuthDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return new AuthDto { Message = "Email or password is incorrect." };

            var token = await CreateJWTToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            var authDto = new AuthDto
            {
                Message = "Login successful!",
                UserName = user.UserName,
                Email = user.Email,
                IsAuthenticated = true,
                Roles = roles.ToList(),
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                TokenExpiration = token.ValidTo
            };

            var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
            if (activeRefreshToken is not null)
            {
                authDto.RefreshToken = activeRefreshToken.Token;
                authDto.RefreshTokenExpiration = activeRefreshToken.Expiration;
            }
            else
            {
                var refreshToken = GenrateRefreshToken();
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
                authDto.RefreshToken = refreshToken.Token;
                authDto.RefreshTokenExpiration = refreshToken.Expiration;
            }

            return authDto;
        }
        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            // Find user with the given refresh token
            var user = await _userManager.Users
                .SingleOrDefaultAsync(r => r.RefreshTokens.Any(t => t.Token == refreshToken));

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken);

            if (user is null || userRefreshToken is null || !userRefreshToken.IsActive)
                return false;

            // Revoke the old refresh token
            userRefreshToken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return true;
        }
        public async Task<string> AddRoleAsync(AddRoleDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null)
                return "Invalid user ID";

            if (!await _roleManager.RoleExistsAsync(model.RoleName))
                return "Invalid role";

            if (await _userManager.IsInRoleAsync(user, model.RoleName))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);
            return result.Succeeded ? string.Empty : "Something went wrong";
        }
        public async Task<AuthDto> RefreshTokenAsync(string refreshToken)
        {
            // Find user with the given refresh token
            var user = await _userManager.Users
                .SingleOrDefaultAsync(r => r.RefreshTokens.Any(t => t.Token == refreshToken));

            if (user is null)
                return new AuthDto { Message = "Invalid Token" };

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken);

            if (userRefreshToken is null || !userRefreshToken.IsActive)
                return new AuthDto { Message = "Invalid Token" };

            // Revoke the old refresh token
            userRefreshToken.RevokedOn = DateTime.UtcNow;

            // Generate and add a new refresh token
            var newRefreshToken = GenrateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            // Create new JWT token
            var newJwtToken = await CreateJWTToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            return new AuthDto
            {
                Message = "Token refreshed successfully!",
                IsAuthenticated = true,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList(),
                Token = new JwtSecurityTokenHandler().WriteToken(newJwtToken),
                TokenExpiration = newJwtToken.ValidTo,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.Expiration
            };
        }
        private async Task<JwtSecurityToken> CreateJWTToken(ApplicationUser user)
        {
            // Gather user claims and roles
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));

            // Build claims array
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            claims.AddRange(userClaims);
            claims.AddRange(roleClaims);

            // Create signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create and return JWT token
            return new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: creds
            );
        }
        private RefreshToken GenrateRefreshToken()
        {
            Span<byte> randomBytes = stackalloc byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                CreatedOn = DateTime.UtcNow,
                Expiration = DateTime.UtcNow.AddDays(7)
            };
        }
    }
}
