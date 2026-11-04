using Google.Apis.Auth;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceAndExpenseTracker.Domain;
using PersonalFinanceAndExpenseTracker.Services.Authentication;
using PersonalFinanceAndExpenseTracker.Services.Models;
using Microsoft.AspNetCore.Authentication.Google;

namespace PersonalFinanceAndExpenseTracker.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleAuthController : ControllerBase
    {
        private readonly ExpenseTrackerDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;
        private readonly IAntiforgery _antiforgery;

        public GoogleAuthController(ExpenseTrackerDbContext context, IConfiguration configuration, TokenService tokenService, IAntiforgery antiforgery)
        {

            _context = context;
            _configuration = configuration;
            _tokenService = tokenService;
            _antiforgery = antiforgery;
        }

        [HttpPost("login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest model)
        {
            var validGoogleUser = await ValidateGoogleToken(model.IdToken);

            if (validGoogleUser == null)
                return Unauthorized();

            var user = _tokenService.FindOrCreateUserByGoogleId(validGoogleUser.JwtId, validGoogleUser.Email, validGoogleUser.GivenName, validGoogleUser.FamilyName, validGoogleUser.Picture);

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

            var accessExpiryMinutes = _configuration.GetValue<int>("JwtSettings:AccessTokenExpiryMinutes");
            var refreshExpiryDays = _configuration.GetValue<int>("JwtSettings:RefreshTokenExpiryDays");

            Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddMinutes(accessExpiryMinutes)
            });

            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
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
                username = user.Username,
                fullName = user.FullName
            });
        }


        private async Task<GoogleJsonWebSignature.Payload?> ValidateGoogleToken(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _configuration["Google:ClientId"] }
            };

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                return payload;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Google Token Validation Failed: " + ex.Message);
                return null;
            }
        }


        [HttpGet("login")]
        public IActionResult Login()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync();

            if (!result.Succeeded)
                return Unauthorized();

            var claims = result.Principal.Claims.Select(c => new
            {
                c.Type,
                c.Value
            });

            return Ok(claims);
        }
    }
}
