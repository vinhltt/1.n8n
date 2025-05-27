using CoreFinance.Application.Services.Base;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CoreFinance.Api.Controllers.Base;

//[Authorize]
public abstract class CrudController<TEntity, TCreateRequest, TUpdateRequest,
    TViewModel, TKey>(
    ILogger logger,
    IBaseService<TEntity, TCreateRequest, TUpdateRequest, TViewModel, TKey>
        baseService
)
    : BaseController(logger)
    where TEntity : BaseEntity<TKey>, new()
    where TCreateRequest : BaseCreateRequest, new()
    where TUpdateRequest : BaseUpdateRequest<TKey>, new()
    where TViewModel : BaseViewModel<TKey>, new()
{
    [HttpPost]
    public virtual async Task<ActionResult> Post([FromForm] TCreateRequest? request)
    {
        if (null == request)
            return BadRequest();
        var result = await baseService.CreateAsync(request);

        if (null == result)
            return StatusCode(StatusCodes.Status500InternalServerError);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public virtual async Task<ActionResult> Put(TKey id, [FromForm] TUpdateRequest request)
    {
        if (!id!.Equals(request.Id))
            return BadRequest();
        try
        {
            return Ok(await baseService.UpdateAsync(id, request));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while updating the entity with ID {Id}", id);
            return StatusCode(500);
        }
    }

    [HttpDelete("{id}")]
    public virtual async Task<ActionResult> Delete(TKey id)
    {
        try
        {
            var result = await baseService.DeleteSoftAsync(id);
            if (result > 0)
                return Ok();
        }
        catch (ArgumentNullException)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpPost("filter")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public abstract Task<ActionResult<IBasePaging<TViewModel>>>
        GetPagingAsync(FilterBodyRequest request);

    [HttpGet("{id}")]
    public virtual async Task<ActionResult> GetById(TKey id)
    {
        var result = await baseService.GetByIdAsync(id);

        if (result != null)
            return Ok(result);
        return StatusCode(500);
    }
}