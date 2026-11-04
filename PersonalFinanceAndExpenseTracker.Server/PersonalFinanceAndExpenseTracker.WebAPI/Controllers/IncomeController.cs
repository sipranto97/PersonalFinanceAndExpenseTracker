using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceAndExpenseTracker.Services;
using PersonalFinanceAndExpenseTracker.Services.Models;

namespace PersonalFinanceAndExpenseTracker.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class IncomeController : ControllerBase
    {
        private readonly IncomeService _incomeService;
        public IncomeController(IncomeService incomeService)
        {
            _incomeService = incomeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateIncome([FromBody] CreateIncomeRequestModel requestDto)
        {
            await _incomeService.CreateIncome(requestDto);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetIncomes()
        {
            var res = await _incomeService.GetIncomes();
            return Ok(res);
        }

        [HttpGet("Categories")]
        public async Task<IActionResult> GetCategories()
        {
            var res = await _incomeService.GetCategories();
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIncome(int id, [FromBody] UpdateIncomeRequestModel requestDto)
        {
            await _incomeService.UpdateIncome(id, requestDto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncome(int id)
        {
            try
            {
                await _incomeService.DeleteIncome(id);
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
