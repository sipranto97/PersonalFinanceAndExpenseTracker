using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PersonalFinanceAndExpenseTracker.Domain;
using PersonalFinanceAndExpenseTracker.Domain.Entities;
using PersonalFinanceAndExpenseTracker.Services;
using PersonalFinanceAndExpenseTracker.Services.Authentication;
using PersonalFinanceAndExpenseTracker.Services.Models;

namespace PersonalFinanceAndExpenseTracker.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly ExpenseTrackerDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;
        private readonly AuthService _authService;
        private readonly IAntiforgery _antiforgery;

        public AuthorizeController(
            ExpenseTrackerDbContext dbContext,
            IConfiguration configuration,
            TokenService tokenService,
            AuthService authService,
            IAntiforgery antiforgery)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tokenService = tokenService;
            _authService = authService;
            _antiforgery = antiforgery;
        }

        [EnableRateLimiting("auth")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserAccount user)
        {
            await _authService.Register(user);
            return Ok();
        }

        [EnableRateLimiting("auth")]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserAccount login)
        {
            var res = await _authService.Login(login);
            return IssueSession(res);
        }

        [EnableRateLimiting("auth")]
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized();
            }

            var res = await _authService.RefreshToken(new TokenModel { RefreshToken = refreshToken });
            return IssueSession(res);
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            _authService.Logout(refreshToken);

            Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
            Response.Cookies.Delete("refresh_token", new CookieOptions { Path = "/api/Authorize" });

            return Ok();
        }

        [HttpGet("CsrfToken")]
        public IActionResult GetCsrfToken()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            return Ok(new { csrfToken = tokens.RequestToken });
        }

        private IActionResult IssueSession(TokenModel tokens)
        {
            var accessExpiryMinutes = _configuration.GetValue<int>("JwtSettings:AccessTokenExpiryMinutes");
            var refreshExpiryDays = _configuration.GetValue<int>("JwtSettings:RefreshTokenExpiryDays");

            Response.Cookies.Append("access_token", tokens.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddMinutes(accessExpiryMinutes)
            });

            Response.Cookies.Append("refresh_token", tokens.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/api/Authorize",
                Expires = DateTimeOffset.UtcNow.AddDays(refreshExpiryDays)
            });

            var csrfTokens = _antiforgery.GetAndStoreTokens(HttpContext);

            return Ok(new
            {
                csrfToken = csrfTokens.RequestToken,
                username = tokens.Username,
                fullName = tokens.FullName
            });
        }
    }
}
