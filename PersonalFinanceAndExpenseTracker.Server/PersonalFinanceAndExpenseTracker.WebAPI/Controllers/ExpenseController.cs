using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceAndExpenseTracker.Services;
using PersonalFinanceAndExpenseTracker.Services.Models;

namespace PersonalFinanceAndExpenseTracker.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly ExpenseService _expenseService;
        public ExpenseController(ExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseRequestModel requestDto)
        {
            await _expenseService.CreateExpense(requestDto);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetExpenses()
        {
            var res = await _expenseService.GetExpenses();
            return Ok(res);
        }

        [HttpGet("Categories")]
        public async Task<IActionResult> GetCategories()
        {
            var res = await _expenseService.GetCategories();
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] UpdateExpenseRequestModel requestDto)
        {
            await _expenseService.UpdateExpense(id, requestDto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            try
            {
                await _expenseService.DeleteExpense(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

    }
}
