using Microsoft.AspNetCore.Mvc;
using CoreFinance.Application.Interfaces;
using CoreFinance.Domain;
using CoreFinance.Api.Controllers.Base;
using CoreFinance.Application.DTOs.Transaction;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.DTOs;

namespace CoreFinance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController(
    ILogger<TransactionController> logger,
    ITransactionService transactionService)
    : CrudController<Transaction, TransactionCreateRequest,
        TransactionUpdateRequest, TransactionViewModel, Guid>(logger,
        transactionService)
{
    [HttpPost("filter")]
    public override async Task<ActionResult<IBasePaging<TransactionViewModel>>> GetPagingAsync(
        FilterBodyRequest request)
    {
        var result = await transactionService.GetPagingAsync(request);
        return Ok(result);
    }
}