using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonalFinanceAndExpenseTracker.Domain;
using PersonalFinanceAndExpenseTracker.Domain.Entities;
using PersonalFinanceAndExpenseTracker.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using PersonalFinanceAndExpenseTracker.Services.Exceptions;
using PersonalFinanceAndExpenseTracker.Services.Models;

namespace PersonalFinanceAndExpenseTracker.Services
{
    public class AuthService
    {
        private readonly TokenService _tokenService;
        private readonly ExpenseTrackerDbContext _dbContext;
        private readonly ILogger<AuthService> _logger;

        public AuthService(TokenService tokenService, ExpenseTrackerDbContext dbContext, ILogger<AuthService> logger)
        {
            _tokenService = tokenService;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Register(UserAccount user)
        {
            var hasher = new PasswordHasher<object>();

            user.Role = "User";
            user.Password = hasher.HashPassword(null, user.Password);

            await _dbContext.UserAccounts.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("New account registered for {Username}", user.Username);
        }
        public async Task<TokenModel> Login(UserAccount login)
        {
            var user = await _dbContext.UserAccounts.FirstOrDefaultAsync(u => u.Username == login.Username);

            if (user == null)
            {
                _logger.LogWarning("Login failed: no account for {Username}", login.Username);
                throw new InvalidCredentialsException("Invalid username or password");
            }

            var hasher = new PasswordHasher<object>();
            var result = hasher.VerifyHashedPassword(null, user.Password, login.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Login failed: bad password for {Username}", login.Username);
                throw new InvalidCredentialsException("Invalid username or password");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

            _logger.LogInformation("Login succeeded for {Username}", login.Username);

            var res = new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Username = user.Username,
                FullName = user.FullName,
            };

            return res;
        }

        public async Task<TokenModel> RefreshToken(TokenModel model)
        {
            var refreshToken = _tokenService.GetRefreshToken(model.RefreshToken);
            if (refreshToken == null)
            {
                throw new InvalidCredentialsException("Token not found.");
            }

            var user = await _dbContext.UserAccounts.FirstOrDefaultAsync(u => u.Id == refreshToken.UserId);
            if (user == null)
            {
                throw new InvalidCredentialsException("User not found.");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id);

            _tokenService.RevokeRefreshToken(refreshToken.RefreshUserToken);

            var res = new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Username = user.Username,
                FullName = user.FullName,
            };

            return res;
        }

        public void Logout(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return;
            }

            _tokenService.RevokeRefreshToken(refreshToken);
        }

    }
}
