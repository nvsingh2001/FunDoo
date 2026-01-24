using System.Security.Claims;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTOs;
using ModelLayer.Utilities;

namespace FunDooApp.Controllers;

[ApiController]
[Authorize]
[Route("api/labels")]
public class LabelController(ILabelService labelService, ILogger<LabelController> logger) : ControllerBase
{
    private int GetUserId()
    {
        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return string.IsNullOrEmpty(value) ? 
            throw new UnauthorizedAccessException("User ID not found in claims.") : 
            int.Parse(value);
    }

    /// <summary>
    /// Create a new Label
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<LabelResponseDto>))]
    public async Task<ActionResult<ApiResponse<LabelResponseDto>>> CreateLabelAsync([FromBody] LabelRequestDto labelDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
            return BadRequest(new ApiResponse<LabelResponseDto>(false, "Validation Error", errors));
        }

        try
        {
            var userId = GetUserId();
            var createdLabel = await labelService.CreateLabelAsync(userId, labelDto);

            return StatusCode(201, new ApiResponse<LabelResponseDto>(true, "Label created successfully", createdLabel));
        }
        catch (Exception ex)
        {
            logger.LogError(ex,"Error creating label: {ExMessage}", ex.Message);
            return StatusCode(500, new ApiResponse<LabelResponseDto>(false, "An error occurred while creating Label"));
        }
    }

    /// <summary>
    /// Get all Labels for the current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<LabelResponseDto>>>> GetAllLabelsAsync()
    {
        var userId = GetUserId();
        var labels = await labelService.GetAllLabelsAsync(userId);
        
        return Ok(new ApiResponse<IEnumerable<LabelResponseDto>>(true, "Labels retrieved successfully", labels));
    }

    /// <summary>
    /// Get all notes associated with a specific label
    /// </summary>
    [HttpGet("{labelId}/notes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IEnumerable<NoteResponseDto>>>> GetNotesByLabelAsync(int labelId)
    {
        try
        {
            var userId = GetUserId();
            var notes = await labelService.GetNotesByLabelAsync(userId, labelId);

            return Ok(new ApiResponse<IEnumerable<NoteResponseDto>>(true, "Notes retrieved successfully", notes));
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError(ex,"label not found: {ExMessage}", ex.Message);
            return NotFound(new ApiResponse<IEnumerable<NoteResponseDto>>(false, ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex,"Error retrieving notes for label: {ExMessage}", ex.Message);
            return StatusCode(500, new ApiResponse<IEnumerable<NoteResponseDto>>(false, "An error occurred while retrieving notes"));
        }
    }

    /// <summary>
    /// Update an existing Label
    /// </summary>
    [HttpPut("{labelId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<LabelResponseDto>>> UpdateLabelAsync(int labelId, [FromBody] LabelRequestDto labelDto)
    {
        try
        {
            var userId = GetUserId();
            var updatedLabel = await labelService.UpdateLabelAsync(userId, labelId, labelDto);

            return Ok(new ApiResponse<LabelResponseDto>(true, "Label updated successfully", updatedLabel));
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError(ex,"Label not found: {ExMessage}", ex.Message);
            return NotFound(new ApiResponse<LabelResponseDto>(false, ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex,"Error updating label: {ExMessage}", ex.Message);
            return StatusCode(500, new ApiResponse<LabelResponseDto>(false, "An error occurred while updating Label"));
        }
    }

    /// <summary>
    /// Delete a Label
    /// </summary>
    [HttpDelete("{labelId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteLabelAsync(int labelId)
    {
        try
        {
            var userId = GetUserId();
            await labelService.DeleteLabelAsync(userId, labelId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError(ex,"Label not found: {ExMessage}", ex.Message);
            return NotFound(new ApiResponse<string>(false, ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex,"Error deleting label: {ExMessage}", ex.Message);
            return StatusCode(500, new ApiResponse<string>(false, "An error occurred while deleting Label"));
        }
    }
}
