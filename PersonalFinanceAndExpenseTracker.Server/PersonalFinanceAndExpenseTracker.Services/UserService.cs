using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain;
using PersonalFinanceAndExpenseTracker.Services.Models;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace PersonalFinanceAndExpenseTracker.Services
{
    public class UserService
    {
        private static readonly string[] AllowedProfilePictureExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxProfilePictureSizeBytes = 5 * 1024 * 1024;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ExpenseTrackerDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserService(IHttpContextAccessor httpContextAccessor, ExpenseTrackerDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public string GetCurrentUserName()
        {
            return _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.Name)?.Value;
        }


        public async Task<UserResponseModel> GetUserInfo()
        {
            var username = GetCurrentUserName();
            var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.Username == username);
            if ( user == null)
            {
                throw new Exception("User Not found.");
            }

            var response = new UserResponseModel
            {
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role,
                Id = user.Id,
                HasPassword = user.Password != null,
                ProfilePictureUrl = user.ProfilePictureUrl,
            };

            return response;
        }

        public async Task UpdateProfile(UpdateProfileRequestModel model)
        {
            var username = GetCurrentUserName();
            var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.FullName = model.FullName;

            await _context.SaveChangesAsync();
        }

        public async Task ChangePassword(ChangePasswordRequestModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NewPassword) || model.NewPassword.Length < 8)
            {
                throw new ArgumentException("New password must be at least 8 characters.");
            }

            var username = GetCurrentUserName();
            var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            if (user.Password == null)
            {
                throw new InvalidOperationException("Password change isn't available for accounts signed in with Google.");
            }

            var hasher = new PasswordHasher<object>();
            var result = hasher.VerifyHashedPassword(null, user.Password, model.CurrentPassword);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new InvalidOperationException("Current password is incorrect.");
            }

            user.Password = hasher.HashPassword(null, model.NewPassword);

            await _context.SaveChangesAsync();
        }

        public async Task<string> UploadProfilePicture(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file was uploaded.");
            }

            if (file.Length > MaxProfilePictureSizeBytes)
            {
                throw new ArgumentException("Profile picture must be 5MB or smaller.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedProfilePictureExtensions.Contains(extension))
            {
                throw new ArgumentException("Profile picture must be a JPG, PNG, or WEBP image.");
            }

            var username = GetCurrentUserName();
            var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var webRootPath = GetWebRootPath();
            var uploadsFolder = Path.Combine(webRootPath, "uploads", "profile-pictures");
            Directory.CreateDirectory(uploadsFolder);

            DeleteLocalProfilePicture(user.ProfilePictureUrl, webRootPath);

            var fileName = $"{user.Id}-{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            user.ProfilePictureUrl = $"/uploads/profile-pictures/{fileName}";

            await _context.SaveChangesAsync();

            return user.ProfilePictureUrl;
        }

        public async Task RemoveProfilePicture()
        {
            var username = GetCurrentUserName();
            var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            DeleteLocalProfilePicture(user.ProfilePictureUrl, GetWebRootPath());

            user.ProfilePictureUrl = null;

            await _context.SaveChangesAsync();
        }

        private string GetWebRootPath()
        {
            return _webHostEnvironment.WebRootPath ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
        }

        private static void DeleteLocalProfilePicture(string? currentUrl, string webRootPath)
        {
            if (string.IsNullOrWhiteSpace(currentUrl) || !currentUrl.StartsWith("/uploads/"))
            {
                return;
            }

            var relativePath = currentUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var existingPath = Path.Combine(webRootPath, relativePath);

            if (File.Exists(existingPath))
            {
                File.Delete(existingPath);
            }
        }
    }
}
