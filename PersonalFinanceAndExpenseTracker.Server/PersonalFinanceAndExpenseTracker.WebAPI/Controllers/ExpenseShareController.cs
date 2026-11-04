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
    public class ExpenseShareController : ControllerBase
    {
        private readonly ExpenseShareService _expenseShareService;
        public ExpenseShareController(ExpenseShareService expenseShareService)
        {
            _expenseShareService = expenseShareService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShareLink([FromBody] CreateExpenseShareLinkRequestModel requestDto)
        {
            var res = await _expenseShareService.CreateShareLink(requestDto);
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpGet("{token}")]
        public async Task<IActionResult> GetSharedExpenseHistory(string token)
        {
            try
            {
                var res = await _expenseShareService.GetSharedExpenseHistory(token);
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
