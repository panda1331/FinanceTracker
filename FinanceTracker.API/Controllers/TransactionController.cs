using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.DTOs.Responses;
using FinanceTracker.Application.Interfaces.Services;
using FinanceTracker.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>Creates a new transaction (income or expense) and updates account balance.</summary>
        /// <param name="request">Account ID, category ID, type, amount, date, optional description.</param>
        [HttpPost]
        public async Task<IActionResult> CreateTransaction(CreateTransactionRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _transactionService.CreateTransactionAsync(userId, request);
            return Ok(ApiResponse<TransactionResponse>.SuccessResponse(response));
        }

        /// <summary>Returns all transactions of the current user.</summary>
        [HttpGet]
        public async Task<IActionResult> GetTransactions()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _transactionService.GetTransactionsByUserIdAsync(userId);
            return Ok(ApiResponse<List<TransactionResponse>>.SuccessResponse(response));
        }

        /// <summary>Deletes a transaction and restores account balance.</summary>
        /// <param name="id">Transaction ID.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _transactionService.DeleteTransactionAsync(userId, id);
            return Ok(ApiResponse<string>.SuccessResponse("Transaction deleted"));
        }

        /// <summary>Updates transaction description.</summary>
        /// <param name="id">Transaction ID.</param>
        /// <param name="request">New description.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] UpdateTransactionRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _transactionService.UpdateTransactionAsync(userId, id, request);
            return Ok(ApiResponse<TransactionResponse>.SuccessResponse(response));
        }
    }
}
