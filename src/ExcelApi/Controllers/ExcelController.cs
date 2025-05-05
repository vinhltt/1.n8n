using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ExcelApi.Services;
using ExcelApi.Models;

namespace ExcelApi.Controllers
{
    /// <summary>
    /// Controller for handling Excel file operations
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the ExcelController
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class ExcelController(IExcelProcessingService excelProcessingService) : ControllerBase
    {

        /// <summary>
        /// Uploads and extract data in Excel file
        /// </summary>
        /// <param name="model">The file upload model containing the Excel file and configuration options</param>
        /// <returns>Processed data from the Excel file</returns>
        /// <response code="200">Returns the processed data</response>
        /// <response code="400">If the file is null or empty</response>
        /// <response code="500">If there was an internal error processing the file</response>
        [HttpPost("extract")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(
            Summary = "Extract data in Excel file",
            Description = "Uploads an Excel file and processes its content. Headers, header row index, and end marker can all be customized."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExtractDataInExcel([FromForm] FileUploadModel model)
        {
            if (model?.File == null || model.File.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                var result = await excelProcessingService.ProcessExcelFileAsync(
                    model.File, 
                    model.Password, 
                    model.Headers,
                    model.HeaderRowIndex,
                    model.EndMarker);
                    
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error processing file: {ex.Message}");
            }
        }
    }
}