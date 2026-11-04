using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceAndExpenseTracker.Services;

namespace PersonalFinanceAndExpenseTracker.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly DashboardService _dashboardService;
        public DashboardController(UserService userService, DashboardService dashboardService)
        {
            _userService = userService;
            _dashboardService = dashboardService;
        }
        [HttpGet("UserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var res = await _userService.GetUserInfo();
            return Ok(res);
        }

        [HttpGet("Balance")]
        public async Task<IActionResult> GetBalance([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var res = await _dashboardService.GetBalance(startDate, endDate);
            return Ok(res);
        }

        [HttpGet("OverviewChart")]
        public async Task<IActionResult> GetOverviewChart([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var res = await _dashboardService.GetOverviewChart(startDate, endDate);
            return Ok(res);
        }

        [HttpGet("ExpenseChart")]
        public async Task<IActionResult> GetExpenseChart([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var res = await _dashboardService.GetExpenseChart(startDate, endDate);
            return Ok(res);
        }

        [HttpGet("IncomeChart")]
        public async Task<IActionResult> GetIncomeChart([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var res = await _dashboardService.GetIncomeChart(startDate, endDate);
            return Ok(res);
        }
    }
}
