using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceAndExpenseTracker.Services;
using PersonalFinanceAndExpenseTracker.Services.Exceptions;
using PersonalFinanceAndExpenseTracker.Services.Models;

namespace PersonalFinanceAndExpenseTracker.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ChartShareController : ControllerBase
    {
        private readonly ChartShareService _chartShareService;
        public ChartShareController(ChartShareService chartShareService)
        {
            _chartShareService = chartShareService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShareLink([FromBody] CreateChartShareLinkRequestModel requestDto)
        {
            var res = await _chartShareService.CreateShareLink(requestDto);
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpGet("{token}")]
        public async Task<IActionResult> GetSharedChartData(string token)
        {
            try
            {
                var res = await _chartShareService.GetSharedChartData(token);
                return Ok(res);
            }
            catch (ShareLinkExpiredException)
            {
                return StatusCode(StatusCodes.Status410Gone, "This share link has expired.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
