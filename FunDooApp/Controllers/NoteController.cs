using System.Security.Claims;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTOs;
using ModelLayer.Utilities;

namespace FunDooApp.Controllers;

[ApiController]
[Authorize]
[Route("api/notes")]
public class NoteController(INoteService noteService, ILogger<NoteController> logger): ControllerBase
{
    private int GetUserId()
    {
        var value = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(value))
        {
            throw new UnauthorizedAccessException("User ID not found in claims.");
        }
        return int.Parse(value);
    }


    /// <summary>
    /// Create Notes
    /// </summary>
    /// <param name="noteCreateDto"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<NoteResponseDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> CreateNoteAsync([FromBody] NoteCreateDto noteCreateDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();

            return BadRequest(new ApiResponse<NoteResponseDto>(
                false,
                "Validation Error",
                errors
            ));
        }
        
        var userId =  GetUserId(); 
        
        try
        {
            var createdNote = await noteService.CreateNoteAsync(userId: userId, noteCreateDto);

            return StatusCode(201,new ApiResponse<NoteResponseDto>(
                    true,
                    "Note created Successfully",
                    createdNote
                )
            );
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning($"Conflict while creating Note: {ex.Message}");
            return Conflict(new ApiResponse<NoteResponseDto>(
                false,
                ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError($"Error creating note: {ex.Message}");
            return StatusCode(500, new ApiResponse<NoteResponseDto>(
                false, 
                "An error occurred while creating Note")
            );
        }
    }
    
    /// <summary>
    /// Get All Notes
    /// </summary>
    /// <param name="isArchive"></param>
    /// <param name="isTrash"></param>
    /// <returns></returns>
    [HttpGet("getUserNotes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<NoteResponseDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> GetAllNotes(bool? isArchive=false, bool? isTrash=false )
    {
        var userId = GetUserId();
        var notes = await noteService.GetAllNotesAsync(userId, isArchive, isTrash);

        return Ok(new ApiResponse<IEnumerable<NoteResponseDto>>(
            true,
            "Notes successfully retrieved",
            notes)
        );
    }

    /// <summary>
    /// Get note by id 
    /// </summary>
    /// <param name="noteId"></param>
    /// <returns></returns>
    [HttpGet("{noteId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<NoteDetailsDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteDetailsDto>>> GetNoteByIdAsync(int noteId)
    {
        try
        {
            var userId = GetUserId();
            var note = await noteService.GetNoteByIdAsync(userId, noteId);

            return Ok(new ApiResponse<NoteDetailsDto>(
                    true,
                    "Note successfully retrieved",
                    note
                )
            );
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning($"Key not found: {ex.Message}");
            return NotFound(new ApiResponse<NoteResponseDto>(
                false,
                ex.Message)
            );
        }
        catch (Exception ex)
        {
            logger.LogError($"Error while retrieving note: {ex.Message}");
            return StatusCode(500, new ApiResponse<NoteDetailsDto>(
                false, 
                "An error occurred while retrieving note")
            );
        }
    }

    [HttpPut("{noteId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<NoteResponseDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> UpdateNoteAsync(int noteId, [FromBody] NoteUpdateDto noteDto)
    {
        try
        {
            var userId =  GetUserId();
            var updatedNote = await noteService.UpdateNoteAsync(userId, noteId,  noteDto);

            return Ok(new ApiResponse<NoteResponseDto>(
                true,
                "Note Updated successfully",
                updatedNote)
            );
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning($"Key not found: {ex.Message}");
            return NotFound(new ApiResponse<NoteDetailsDto>(
                false,
                ex.Message)
            );
        }
        catch (Exception ex)
        {
            logger.LogError($"Error while updating note: {ex.Message}");
            return StatusCode(500, new ApiResponse<NoteDetailsDto>(
                false, 
                "An error occurred while updating note")
            );
        }
    }
    
    [HttpPatch("{noteId}/archive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<NoteResponseDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> ToggleArchiveAsync(int noteId)
    {
        try
        {
            var userId = GetUserId();
            var updatedNote = await noteService.ToggleArchiveAsync(userId, noteId);

            return Ok(new ApiResponse<NoteResponseDto>(
                true,
                "Note archive status updated successfully",
                updatedNote
            ));
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning($"Key not found: {ex.Message}");
            return NotFound(new ApiResponse<NoteResponseDto>(
                false,
                ex.Message)
            );
        }
        catch (Exception ex)
        {
            logger.LogError($"Error while archiving note: {ex.Message}");
            return StatusCode(500, new ApiResponse<NoteResponseDto>(
                false, 
                "An error occurred while updating archive status")
            );
        }
    }

    [HttpPatch("{noteId}/pin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<NoteResponseDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> TogglePinAsync(int noteId, [FromBody] NotePinDto pinDto)
    {
        try
        {
            var userId = GetUserId();
            var updatedNote = await noteService.TogglePinAsync(userId, noteId, pinDto);

            return Ok(new ApiResponse<NoteResponseDto>(
                true,
                "Note pin status updated successfully",
                updatedNote
            ));
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning($"Key not found: {ex.Message}");
            return NotFound(new ApiResponse<NoteResponseDto>(
                false,
                ex.Message)
            );
        }
        catch (Exception ex)
        {
            logger.LogError($"Error while pinning note: {ex.Message}");
            return StatusCode(500, new ApiResponse<NoteResponseDto>(
                false, 
                "An error occurred while updating pin status")
            );
        }
    }
    
    /// <summary>
    /// Restore Soft Delete Note
    /// </summary>
    /// <param name="noteId"></param>
    /// <returns></returns>
    [HttpPatch("{noteId}/restore")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<NoteResponseDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> RestoreNoteAsync(int noteId)
    {
        try
        {
            var userId = GetUserId();
            var restoredNote = await noteService.RestoreNoteAsync(userId, noteId);

            return Ok(new ApiResponse<NoteResponseDto>(
                true,
                "Note restored successfully",
                restoredNote
            ));
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning($"Key not found: {ex.Message}");
            return NotFound(new ApiResponse<NoteResponseDto>(
                false,
                ex.Message)
            );
        }
        catch (Exception ex)
        {
            logger.LogError($"Error while restoring note: {ex.Message}");
            return StatusCode(500, new ApiResponse<NoteResponseDto>(
                false, 
                "An error occurred while restoring note")
            );
        }
    }
    
    /// <summary>
    /// Get all the archived notes
    /// </summary>
    /// <returns></returns>
    [HttpGet("archive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> GetArchivedNotes()
    {
        return await GetAllNotes(isArchive: true, isTrash: false);
    }
    
    /// <summary>
    /// Get All the Thrashed Notes
    /// </summary>
    /// <returns></returns>
    [HttpGet("trash")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> GetTrashedNotes()
    {
        return await GetAllNotes(isArchive: false, isTrash: true);
    }

    /// <summary>
    /// Update the Color
    /// </summary>
    /// <param name="noteId"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    [HttpPatch("{noteId}/color")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<NoteResponseDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> UpdateNoteColorAsync(int noteId, [FromBody] string color)
    {
        try
        {
            var userId = GetUserId();
            var updatedNote = await noteService.UpdateColorAsync(userId, noteId, color);

            return Ok(new ApiResponse<NoteResponseDto>(
                true,
                "Note color updated successfully",
                updatedNote
            ));
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning($"Key not found: {ex.Message}");
            return NotFound(new ApiResponse<NoteResponseDto>(
                false,
                ex.Message)
            );
        }
        catch (Exception ex)
        {
            logger.LogError($"Error while updating note color: {ex.Message}");
            return StatusCode(500, new ApiResponse<NoteResponseDto>(
                false, 
                "An error occurred while updating note color")
            );
        }
    }
    
    /// <summary>
    /// Delete Note Soft
    /// </summary>
    /// <param name="noteId"></param>
    /// <returns></returns>
    [HttpDelete("{noteId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<NoteResponseDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> DeleteNoteAsync(int noteId)
    {
        try
        {
            var userId =  GetUserId();
            await noteService.DeleteNoteAsync(userId, noteId);
            
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning($"Key not found: {ex.Message}");
            return NotFound(new ApiResponse<NoteResponseDto>(
                false,
                ex.Message)
            );
        }
        catch (Exception ex)
        {
            logger.LogError($"Error while deleting note: {ex.Message}");
            return StatusCode(500, new ApiResponse<NoteResponseDto>(
                false, 
                "An error occurred while deleting note")
            );
        }
    }
    
    
    /// <summary>
    /// Delete Note Permanently 
    /// </summary>
    /// <param name="noteId"></param>
    /// <returns></returns>
    [HttpDelete("{noteId:int}/Permanent")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<NoteResponseDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> DeleteNotePermanentlyAsync(int noteId)
    {
        try
        {
            var userId =  GetUserId();
            await noteService.DeleteNotePermanentlyAsync(userId, noteId);
            
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning($"Key not found: {ex.Message}");
            return NotFound(new ApiResponse<NoteResponseDto>(
                false,
                ex.Message)
            );
        }
        catch (Exception ex)
        {
            logger.LogError($"Error while deleting note: {ex.Message}");
            return StatusCode(500, new ApiResponse<NoteResponseDto>(
                false, 
                "An error occurred while deleting note")
            );
        }
    }
}