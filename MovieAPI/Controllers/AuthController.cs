using MovieAPI.Services.@interface;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterAuth model)
        {
            var result = await _authService.Register(model);
            if(!result.IsAuthenticated)
                return BadRequest(result.Massage);
            return Ok(result);
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] TokenRequestModel model)
        {
            var result = await _authService.Login(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Massage);

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }
        
        [HttpPost("addrole")]
        public async Task<IActionResult> AddRole([FromBody] AddRoleModel model)
        {
            var result = await _authService.AddRole(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);
            return Ok(model);
        }


        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _authService.RefreshToken(refreshToken);

            if (!result.IsAuthenticated)
                return BadRequest(result);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeToken model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required!");

            var result = await _authService.RevokeToken(token);

            if (!result)
                return BadRequest("Token is invalid!");

            return Ok();
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
