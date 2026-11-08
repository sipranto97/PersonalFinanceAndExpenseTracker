using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain;
using PersonalFinanceAndExpenseTracker.Domain.Entities;
using PersonalFinanceAndExpenseTracker.Services.Exceptions;
using PersonalFinanceAndExpenseTracker.Services.Models;
using System.Security.Cryptography;

namespace PersonalFinanceAndExpenseTracker.Services
{
    public class ChartShareService
    {
        private readonly ExpenseTrackerDbContext _context;
        private readonly UserService _userService;
        private readonly DashboardService _dashboardService;

        public ChartShareService(ExpenseTrackerDbContext context, UserService userService, DashboardService dashboardService)
        {
            _context = context;
            _userService = userService;
            _dashboardService = dashboardService;
        }

        public async Task<ChartShareLinkResponseModel> CreateShareLink(CreateChartShareLinkRequestModel model)
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

            var shareLink = new ChartShareLink
            {
                Token = RandomNumberGenerator.GetHexString(32),
                ChartType = model.ChartType,
                StartDate = model.StartDate.Date,
                EndDate = model.EndDate.Date,
                ExpiresAt = model.ExpiresAt?.Date,
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id
            };

            await _context.ChartShareLinks.AddAsync(shareLink);
            await _context.SaveChangesAsync();

            return new ChartShareLinkResponseModel
            {
                Token = shareLink.Token
            };
        }

        public async Task<SharedChartDataResponseModel> GetSharedChartData(string token)
        {
            var shareLink = await _context.ChartShareLinks.FirstOrDefaultAsync(x => x.Token == token);

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

            var response = new SharedChartDataResponseModel
            {
                OwnerName = !string.IsNullOrWhiteSpace(owner.FullName) ? owner.FullName : owner.Username,
                Currency = owner.Currency,
                ChartType = shareLink.ChartType,
                StartDate = shareLink.StartDate,
                EndDate = shareLink.EndDate,
                ExpiresAt = shareLink.ExpiresAt
            };

            switch (shareLink.ChartType)
            {
                case ChartType.Overview:
                    response.Overview = await _dashboardService.GetOverviewChart(shareLink.UserId, shareLink.StartDate, shareLink.EndDate);
                    break;
                case ChartType.Expense:
                    response.Expense = await _dashboardService.GetExpenseChart(shareLink.UserId, shareLink.StartDate, shareLink.EndDate);
                    break;
                case ChartType.Income:
                    response.Income = await _dashboardService.GetIncomeChart(shareLink.UserId, shareLink.StartDate, shareLink.EndDate);
                    break;
            }

            return response;
        }
    }
}
