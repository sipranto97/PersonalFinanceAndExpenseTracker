using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceAndExpenseTracker.Services;
using PersonalFinanceAndExpenseTracker.Services.Models;

namespace PersonalFinanceAndExpenseTracker.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RecurringIncomeController : ControllerBase
    {
        private readonly RecurringIncomeService _recurringIncomeService;
        public RecurringIncomeController(RecurringIncomeService recurringIncomeService)
        {
            _recurringIncomeService = recurringIncomeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecurringIncome([FromBody] CreateRecurringIncomeRequestModel requestDto)
        {
            await _recurringIncomeService.CreateRecurringIncome(requestDto);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetRecurringIncomes()
        {
            var res = await _recurringIncomeService.GetRecurringIncomes();
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecurringIncome(int id, [FromBody] UpdateRecurringIncomeRequestModel requestDto)
        {
            await _recurringIncomeService.UpdateRecurringIncome(id, requestDto);
            return Ok();
        }

        [HttpPost("{id}/Cancel")]
        public async Task<IActionResult> CancelRecurringIncome(int id)
        {
            await _recurringIncomeService.CancelRecurringIncome(id);
            return Ok();
        }
    }
}
