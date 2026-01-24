using System.Security.Claims;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTOs;
using ModelLayer.Utilities;

namespace FunDooApp.Controllers;

[ApiController]
[Authorize]
[Route("api/notes/{noteId}/collaborators")]
public class CollaboratorController(
    ICollaboratorService collaboratorService, 
    ILogger<CollaboratorController> logger
    ) : ControllerBase
{
    private int GetUserId()
    {
        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return string.IsNullOrEmpty(value) ? 
            throw new UnauthorizedAccessException("User ID not found in claims.") : 
            int.Parse(value);
    }

    /// <summary>
    /// Add a collaborator to a note
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, 
        Type = typeof(ApiResponse<CollaboratorResponseDto>))]
    public async Task<ActionResult<ApiResponse<CollaboratorResponseDto>>> 
        AddCollaboratorAsync(int noteId, [FromBody] CollaboratorRequestDto collaboratorDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<CollaboratorResponseDto>(
                    false,
                    "Validation Error",
                    ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray()
                )
            );
        }

        try
        {
            var userId = GetUserId();
            var response = await collaboratorService.AddCollaboratorAsync(userId, noteId, collaboratorDto);

            return StatusCode(201, new ApiResponse<CollaboratorResponseDto>(
                    true,
                    "Collaborator added successfully",
                    response
                )
            );
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid operation: {ExMessage}", ex.Message);
            return BadRequest(new ApiResponse<CollaboratorResponseDto>(false, ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex,"Key not found: {ExMessage}", ex.Message);
            return NotFound(new ApiResponse<CollaboratorResponseDto>(
                    false,
                    ex.Message
                )
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding collaborator: {ExMessage}", ex.Message);
            return StatusCode(500, new ApiResponse<CollaboratorResponseDto>(
                    false,
                    "An error occurred while adding collaborator"
                )
            );
        }
    }

    /// <summary>
    /// Get all collaborators for a note
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CollaboratorResponseDto>>>> GetCollaboratorsAsync(int noteId)
    {
        try
        {
            var userId = GetUserId();
            var response = await collaboratorService.GetCollaboratorsAsync(userId, noteId);
            
            return Ok(new ApiResponse<IEnumerable<CollaboratorResponseDto>>(true, "Collaborators retrieved successfully", response));
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError(ex,"Key not found: {ExMessage}", ex.Message);
            return NotFound(new ApiResponse<IEnumerable<CollaboratorResponseDto>>(false, ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex,"Error retrieving collaborators: {ExMessage}", ex.Message);
            return StatusCode(500, new ApiResponse<IEnumerable<CollaboratorResponseDto>>(
                    false,
                    "An error occurred while retrieving collaborators"
                )
            );
        }
    }

    /// <summary>
    /// Remove a collaborator from a note
    /// </summary>
    [HttpDelete("{collaboratorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<string>>> RemoveCollaboratorAsync(int noteId, int collaboratorId)
    {
        try
        {
            var userId = GetUserId();
            await collaboratorService.RemoveCollaboratorAsync(userId, noteId, collaboratorId);
            
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError(ex,"Key not found: {ExMessage}", ex.Message);
            return NotFound(new ApiResponse<string>(false, ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing collaborator: {ExMessage}", ex.Message);
            return StatusCode(500, new ApiResponse<string>(
                    false,
                    "An error occurred while removing collaborator"
                )
            );
        }
    }
}
