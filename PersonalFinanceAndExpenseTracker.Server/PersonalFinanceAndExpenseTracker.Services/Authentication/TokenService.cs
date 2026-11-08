using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PersonalFinanceAndExpenseTracker.Domain;
using PersonalFinanceAndExpenseTracker.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PersonalFinanceAndExpenseTracker.Services.Authentication
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ExpenseTrackerDbContext _context;

        public TokenService(IConfiguration configuration, ExpenseTrackerDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public string GenerateAccessToken(UserAccount user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:AccessTokenExpiryMinutes"]));

            var token = new JwtSecurityToken(

                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken(int userId)
        {
            var tokenId = Guid.NewGuid().ToString();
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            // Set the expiry for the refresh token
            var expiryDays = Convert.ToInt32(_configuration["JwtSettings:RefreshTokenExpiryDays"]);
            var expiryDate = DateTime.UtcNow.AddDays(expiryDays);

            var token = new RefreshToken
            {
                UserId = userId,
                TokenId = tokenId,
                RefreshUserToken = refreshToken,
                ExpiresAt = expiryDate
            };

            _context.RefreshTokens.Add(token);
            _context.SaveChanges();

            return refreshToken;
        }

        public RefreshToken GetRefreshToken(string refreshToken)
        {
            var token = _context.RefreshTokens
                .FirstOrDefault(rt => rt.RefreshUserToken == refreshToken);

            if (token == null)
            {
                return null;
            }

            if (token.ExpiresAt < DateTime.UtcNow)
            {
                _context.RefreshTokens.Remove(token);
                _context.SaveChanges();
                return null;
            }

            return token;
        }

        public void RevokeRefreshToken(string refreshToken)
        {
            var token = _context.RefreshTokens.FirstOrDefault(rt => rt.RefreshUserToken == refreshToken);
            if (token != null)
            {
                _context.RefreshTokens.Remove(token);
                _context.SaveChanges();
            }
        }
        public UserAccount FindOrCreateUserByGoogleId(string googleId, string email, string fName, string lName, string pictureUrl, string? currencyHint = null)
        {
            var user = _context.UserAccounts.FirstOrDefault(u => u.Username == email);

            if (user == null)
            {
                var newUser = new UserAccount
                {
                    FullName = $"{fName} {lName}",
                    GoogleId = googleId,
                    Username = email,
                    Role = "User",
                    ProfilePictureUrl = pictureUrl,
                    // Google sign-in has no sign-up form, so the currency is a guess from
                    // the browser locale. The user confirms or changes it on first visit.
                    Currency = SupportedCurrencies.Normalize(currencyHint),
                    NeedsCurrencySetup = true
                };
                _context.UserAccounts.Add(newUser);
                _context.SaveChanges();
                return newUser;
            }

            if (user.GoogleId == null)
            {
                user.GoogleId = googleId;
                _context.SaveChanges();
            }

            return user;
        }

    }
}
