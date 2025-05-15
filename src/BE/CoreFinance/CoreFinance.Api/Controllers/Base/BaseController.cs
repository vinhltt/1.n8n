using Microsoft.AspNetCore.Mvc;

namespace CoreFinance.Api.Controllers.Base;

/// <summary>
/// Base controller for API controllers, provides common functionality.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public abstract class BaseController(
    ILogger logger
) : ControllerBase
{
    public readonly ILogger Logger = logger;
}