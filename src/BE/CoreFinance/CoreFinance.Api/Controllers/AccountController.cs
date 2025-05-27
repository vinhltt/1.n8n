using Microsoft.AspNetCore.Mvc;
using CoreFinance.Application.Interfaces;
using CoreFinance.Domain;
using CoreFinance.Api.Controllers.Base;
using CoreFinance.Application.DTOs.Account;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.DTOs;

namespace CoreFinance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(
    ILogger<AccountController> logger,
    IAccountService accountService)
    : CrudController<Account, AccountCreateRequest,
        AccountUpdateRequest, AccountViewModel, Guid>(logger,
        accountService)
{
    [HttpPost("filter")]
    public override async Task<ActionResult<IBasePaging<AccountViewModel>>> GetPagingAsync(FilterBodyRequest request)
    {
        var result = await accountService.GetPagingAsync(request);
        return Ok(result);
    }
}