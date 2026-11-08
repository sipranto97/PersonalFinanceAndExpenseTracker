using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain;
using PersonalFinanceAndExpenseTracker.Domain.Entities;
using PersonalFinanceAndExpenseTracker.Services.Exceptions;
using PersonalFinanceAndExpenseTracker.Services.Models;
using System.Security.Cryptography;

namespace PersonalFinanceAndExpenseTracker.Services
{
    public class IncomeShareService
    {
        private readonly ExpenseTrackerDbContext _context;
        private readonly UserService _userService;

        public IncomeShareService(ExpenseTrackerDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<ShareLinkResponseModel> CreateShareLink(CreateShareLinkRequestModel model)
        {
            if (model.StartDate.Date > model.EndDate.Date)
            {
                throw new ArgumentException("Start date must be before the end date.");
            }

            if (model.ExpiresAt.HasValue && model.ExpiresAt.Value.Date < DateTime.UtcNow.Date)
            {
                throw new ArgumentException("Expiration date must be today or later.");
            }

            var user = await _userService.GetUserInfo();

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var shareLink = new IncomeShareLink
            {
                Token = RandomNumberGenerator.GetHexString(32),
                StartDate = model.StartDate.Date,
                EndDate = model.EndDate.Date,
                ExpiresAt = model.ExpiresAt?.Date,
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id
            };

            await _context.IncomeShareLinks.AddAsync(shareLink);
            await _context.SaveChangesAsync();

            return new ShareLinkResponseModel
            {
                Token = shareLink.Token
            };
        }

        public async Task<SharedIncomeHistoryResponseModel> GetSharedIncomeHistory(string token)
        {
            var shareLink = await _context.IncomeShareLinks.FirstOrDefaultAsync(x => x.Token == token);

            if (shareLink == null)
            {
                throw new KeyNotFoundException("Share link not found");
            }

            if (shareLink.ExpiresAt.HasValue && DateTime.UtcNow.Date > shareLink.ExpiresAt.Value.Date)
            {
                throw new ShareLinkExpiredException("This share link has expired.");
            }

            var owner = await _context.UserAccounts.FirstOrDefaultAsync(x => x.Id == shareLink.UserId);

            if (owner == null)
            {
                throw new KeyNotFoundException("Share link not found");
            }

            var incomes = await _context.Incomes
                .Include(x => x.Category)
                .Where(x => x.UserId == shareLink.UserId
                    && x.TrxDate.Date >= shareLink.StartDate.Date
                    && x.TrxDate.Date <= shareLink.EndDate.Date)
                .OrderBy(x => x.TrxDate)
                .Select(x => new SharedIncomeItemModel
                {
                    Category = x.Category.Name,
                    Amount = x.Amount,
                    TrxDate = x.TrxDate
                })
                .ToListAsync();

            return new SharedIncomeHistoryResponseModel
            {
                OwnerName = !string.IsNullOrWhiteSpace(owner.FullName) ? owner.FullName : owner.Username,
                StartDate = shareLink.StartDate,
                EndDate = shareLink.EndDate,
                Currency = owner.Currency,
                ExpiresAt = shareLink.ExpiresAt,
                Incomes = incomes
            };
        }
    }
}
