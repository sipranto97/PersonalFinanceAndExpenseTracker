using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceAndExpenseTracker.Services;
using PersonalFinanceAndExpenseTracker.Services.Models;

namespace PersonalFinanceAndExpenseTracker.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RecurringExpenseController : ControllerBase
    {
        private readonly RecurringExpenseService _recurringExpenseService;
        public RecurringExpenseController(RecurringExpenseService recurringExpenseService)
        {
            _recurringExpenseService = recurringExpenseService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecurringExpense([FromBody] CreateRecurringExpenseRequestModel requestDto)
        {
            await _recurringExpenseService.CreateRecurringExpense(requestDto);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetRecurringExpenses()
        {
            var res = await _recurringExpenseService.GetRecurringExpenses();
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecurringExpense(int id, [FromBody] UpdateRecurringExpenseRequestModel requestDto)
        {
            await _recurringExpenseService.UpdateRecurringExpense(id, requestDto);
            return Ok();
        }

        [HttpPost("{id}/Cancel")]
        public async Task<IActionResult> CancelRecurringExpense(int id)
        {
            await _recurringExpenseService.CancelRecurringExpense(id);
            return Ok();
        }
    }
}
