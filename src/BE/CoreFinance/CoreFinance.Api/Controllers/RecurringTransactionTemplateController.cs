using CoreFinance.Api.Controllers.Base;
using CoreFinance.Application.DTOs.RecurringTransactionTemplate;
using CoreFinance.Application.Interfaces;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.DTOs;
using CoreFinance.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CoreFinance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecurringTransactionTemplateController(
    IRecurringTransactionTemplateService service,
    ILogger<RecurringTransactionTemplateController> logger)
    : CrudController<RecurringTransactionTemplate, RecurringTransactionTemplateCreateRequest,
        RecurringTransactionTemplateUpdateRequest, RecurringTransactionTemplateViewModel, Guid>(logger, service)
{
    /// <summary>
    ///     Get paginated recurring transaction templates (EN)
    ///     Lấy danh sách mẫu giao dịch định kỳ có phân trang (VI)
    /// </summary>
    [HttpPost("filter")]
    public override async Task<ActionResult<IBasePaging<RecurringTransactionTemplateViewModel>>> GetPagingAsync(
        [FromBody] FilterBodyRequest request)
    {
        var result = await service.GetPagingAsync(request);
        return Ok(result);
    }

    /// <summary>
    ///     Get active templates for a user (EN)
    ///     Lấy các mẫu đang hoạt động của người dùng (VI)
    /// </summary>
    [HttpGet("active/{userId:guid}")]
    public async Task<IActionResult> GetActiveTemplates(Guid userId)
    {
        var result = await service.GetActiveTemplatesAsync(userId);
        return Ok(result);
    }

    /// <summary>
    ///     Get templates by account (EN)
    ///     Lấy mẫu theo tài khoản (VI)
    /// </summary>
    [HttpGet("account/{accountId:guid}")]
    public async Task<IActionResult> GetTemplatesByAccount(Guid accountId)
    {
        var result = await service.GetTemplatesByAccountAsync(accountId);
        return Ok(result);
    }

    /// <summary>
    ///     Toggle active status of a template (EN)
    ///     Bật/tắt trạng thái hoạt động của mẫu (VI)
    /// </summary>
    [HttpPatch("{templateId:guid}/toggle-active")]
    public async Task<IActionResult> ToggleActiveStatus(Guid templateId, [FromBody] bool isActive)
    {
        var result = await service.ToggleActiveStatusAsync(templateId, isActive);
        if (!result)
            return NotFound("Template not found");

        return Ok(new { success = true, message = "Template status updated successfully" });
    }

    /// <summary>
    ///     Calculate next execution date for a template (EN)
    ///     Tính ngày thực hiện tiếp theo cho mẫu (VI)
    /// </summary>
    [HttpGet("{templateId:guid}/next-execution-date")]
    public async Task<IActionResult> CalculateNextExecutionDate(Guid templateId)
    {
        try
        {
            var nextDate = await service.CalculateNextExecutionDateAsync(templateId);
            return Ok(new { nextExecutionDate = nextDate });
        }
        catch (ArgumentException)
        {
            return NotFound("Template not found");
        }
    }

    /// <summary>
    ///     Generate expected transactions for a template (EN)
    ///     Sinh giao dịch dự kiến cho mẫu (VI)
    /// </summary>
    [HttpPost("{templateId:guid}/generate-expected-transactions")]
    public async Task<IActionResult> GenerateExpectedTransactions(Guid templateId, [FromQuery] int daysInAdvance = 30)
    {
        try
        {
            await service.GenerateExpectedTransactionsAsync(templateId, daysInAdvance);
            return Ok(new { success = true, message = "Expected transactions generated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    ///     Generate expected transactions for all active templates (EN)
    ///     Sinh giao dịch dự kiến cho tất cả mẫu đang hoạt động (VI)
    /// </summary>
    [HttpPost("generate-all-expected-transactions")]
    public async Task<IActionResult> GenerateExpectedTransactionsForAllActiveTemplates()
    {
        try
        {
            await service.GenerateExpectedTransactionsForAllActiveTemplatesAsync();
            return Ok(new { success = true, message = "Expected transactions generated for all active templates" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}