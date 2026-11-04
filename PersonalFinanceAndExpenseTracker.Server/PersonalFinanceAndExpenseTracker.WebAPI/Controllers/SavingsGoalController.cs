using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceAndExpenseTracker.Services;
using PersonalFinanceAndExpenseTracker.Services.Models;

namespace PersonalFinanceAndExpenseTracker.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class SavingsGoalController : ControllerBase
    {
        private readonly SavingsGoalService _savingsGoalService;
        public SavingsGoalController(SavingsGoalService savingsGoalService)
        {
            _savingsGoalService = savingsGoalService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGoal([FromBody] CreateSavingsGoalRequestModel requestDto)
        {
            var res = await _savingsGoalService.CreateGoal(requestDto);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetGoals()
        {
            var res = await _savingsGoalService.GetGoals();
            return Ok(res);
        }

        [HttpGet("Active")]
        public async Task<IActionResult> GetActiveGoals()
        {
            var res = await _savingsGoalService.GetActiveGoals();
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGoal(int id, [FromBody] UpdateSavingsGoalRequestModel requestDto)
        {
            await _savingsGoalService.UpdateGoal(id, requestDto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            await _savingsGoalService.DeleteGoal(id);
            return Ok();
        }

        [HttpPost("{id}/Contribute")]
        public async Task<IActionResult> AddContribution(int id, [FromBody] AddContributionRequestModel requestDto)
        {
            try
            {
                var res = await _savingsGoalService.AddContribution(id, requestDto);
                return Ok(res);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("{id}/Settle")]
        public async Task<IActionResult> SettleGoal(int id, [FromBody] SettleGoalRequestModel requestDto)
        {
            var res = await _savingsGoalService.SettleGoal(id, requestDto);
            return Ok(res);
        }
    }
}
