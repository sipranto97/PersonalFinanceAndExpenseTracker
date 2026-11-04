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
    public class IncomeShareController : ControllerBase
    {
        private readonly IncomeShareService _incomeShareService;
        public IncomeShareController(IncomeShareService incomeShareService)
        {
            _incomeShareService = incomeShareService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShareLink([FromBody] CreateShareLinkRequestModel requestDto)
        {
            var res = await _incomeShareService.CreateShareLink(requestDto);
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpGet("{token}")]
        public async Task<IActionResult> GetSharedIncomeHistory(string token)
        {
            try
            {
                var res = await _incomeShareService.GetSharedIncomeHistory(token);
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
