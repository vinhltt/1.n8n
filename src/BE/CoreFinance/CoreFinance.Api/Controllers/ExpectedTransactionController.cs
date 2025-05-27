using CoreFinance.Api.Controllers.Base;
using CoreFinance.Application.DTOs.ExpectedTransaction;
using CoreFinance.Application.Interfaces;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.DTOs;
using CoreFinance.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CoreFinance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpectedTransactionController(
    IExpectedTransactionService service,
    ILogger<ExpectedTransactionController> logger)
    : CrudController<ExpectedTransaction, ExpectedTransactionCreateRequest,
        ExpectedTransactionUpdateRequest, ExpectedTransactionViewModel, Guid>(logger, service)
{
    /// <summary>
    ///     Get paginated expected transactions (EN)
    ///     Lấy danh sách giao dịch dự kiến có phân trang (VI)
    /// </summary>
    [HttpPost("filter")]
    public override async Task<ActionResult<IBasePaging<ExpectedTransactionViewModel>>> GetPagingAsync(
        [FromBody] FilterBodyRequest request)
    {
        var result = await service.GetPagingAsync(request);
        return Ok(result);
    }

    /// <summary>
    ///     Get pending transactions for a user (EN)
    ///     Lấy giao dịch đang chờ của người dùng (VI)
    /// </summary>
    [HttpGet("pending/{userId:guid}")]
    public async Task<IActionResult> GetPendingTransactionsAsync(Guid userId)
    {
        var result = await service.GetPendingTransactionsAsync(userId);
        return Ok(result);
    }

    /// <summary>
    ///     Get upcoming transactions for a user (EN)
    ///     Lấy giao dịch sắp tới của người dùng (VI)
    /// </summary>
    [HttpGet("upcoming/{userId:guid}")]
    public async Task<IActionResult> GetUpcomingTransactions(Guid userId, [FromQuery] int days = 30)
    {
        var result = await service.GetUpcomingTransactionsAsync(userId, days);
        return Ok(result);
    }

    /// <summary>
    ///     Get transactions by template (EN)
    ///     Lấy giao dịch theo mẫu (VI)
    /// </summary>
    [HttpGet("template/{templateId:guid}")]
    public async Task<IActionResult> GetTransactionsByTemplate(Guid templateId)
    {
        var result = await service.GetTransactionsByTemplateAsync(templateId);
        return Ok(result);
    }

    /// <summary>
    ///     Get transactions by account (EN)
    ///     Lấy giao dịch theo tài khoản (VI)
    /// </summary>
    [HttpGet("account/{accountId:guid}")]
    public async Task<IActionResult> GetTransactionsByAccount(Guid accountId)
    {
        var result = await service.GetTransactionsByAccountAsync(accountId);
        return Ok(result);
    }

    /// <summary>
    ///     Get transactions by date range (EN)
    ///     Lấy giao dịch theo khoảng thời gian (VI)
    /// </summary>
    [HttpGet("date-range/{userId:guid}")]
    public async Task<IActionResult> GetTransactionsByDateRange(Guid userId, [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var result = await service.GetTransactionsByDateRangeAsync(userId, startDate, endDate);
        return Ok(result);
    }

    /// <summary>
    ///     Confirm an expected transaction (EN)
    ///     Xác nhận giao dịch dự kiến (VI)
    /// </summary>
    [HttpPost("{expectedTransactionId:guid}/confirm")]
    public async Task<IActionResult> ConfirmExpectedTransaction(Guid expectedTransactionId,
        [FromBody] ConfirmTransactionRequest request)
    {
        var result = await service.ConfirmExpectedTransactionAsync(expectedTransactionId, request.ActualTransactionId);
        if (!result)
            return NotFound("Expected transaction not found or cannot be confirmed");

        return Ok(new { success = true, message = "Expected transaction confirmed successfully" });
    }

    /// <summary>
    ///     Cancel an expected transaction (EN)
    ///     Hủy giao dịch dự kiến (VI)
    /// </summary>
    [HttpPost("{expectedTransactionId:guid}/cancel")]
    public async Task<IActionResult> CancelExpectedTransaction(Guid expectedTransactionId,
        [FromBody] CancelTransactionRequest request)
    {
        var result = await service.CancelExpectedTransactionAsync(expectedTransactionId, request.Reason);
        if (!result)
            return NotFound("Expected transaction not found or cannot be cancelled");

        return Ok(new { success = true, message = "Expected transaction cancelled successfully" });
    }

    /// <summary>
    ///     Adjust an expected transaction amount (EN)
    ///     Điều chỉnh số tiền giao dịch dự kiến (VI)
    /// </summary>
    [HttpPost("{expectedTransactionId:guid}/adjust")]
    public async Task<IActionResult> AdjustExpectedTransaction(Guid expectedTransactionId,
        [FromBody] AdjustTransactionRequest request)
    {
        var result =
            await service.AdjustExpectedTransactionAsync(expectedTransactionId, request.NewAmount, request.Reason);
        if (!result)
            return NotFound("Expected transaction not found or cannot be adjusted");

        return Ok(new { success = true, message = "Expected transaction adjusted successfully" });
    }

    /// <summary>
    ///     Get cash flow forecast (EN)
    ///     Lấy dự báo dòng tiền (VI)
    /// </summary>
    [HttpGet("cash-flow-forecast/{userId:guid}")]
    public async Task<IActionResult> GetCashFlowForecast(Guid userId, [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var result = await service.GetCashFlowForecastAsync(userId, startDate, endDate);
        return Ok(new { cashFlowForecast = result, startDate, endDate });
    }

    /// <summary>
    ///     Get category forecast (EN)
    ///     Lấy dự báo theo danh mục (VI)
    /// </summary>
    [HttpGet("category-forecast/{userId:guid}")]
    public async Task<IActionResult> GetCategoryForecast(Guid userId, [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var result = await service.GetCategoryForecastAsync(userId, startDate, endDate);
        return Ok(new { categoryForecast = result, startDate, endDate });
    }
}