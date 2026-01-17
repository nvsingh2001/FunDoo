using ModelLayer.DTOs;
using ModelLayer.Utilities;

namespace BusinessLogicLayer.Interfaces;

public interface INoteService
{
    Task<ApiResponse<NoteResponseDto>> CreateNoteAsync(int userId, NoteCreateDto noteDto);
    Task<ApiResponse<IEnumerable<NoteResponseDto>>> GetAllNotesAsync(int userId, bool? isArchived = null, bool? isTrashed = null);
    Task<ApiResponse<NoteDetailsDto>> GetNoteByIdAsync(int userId, int noteId);
    Task<ApiResponse<NoteResponseDto>> UpdateNoteAsync(int userId, int noteId, NoteUpdateDto noteDto);
    Task<ApiResponse<string>> DeleteNoteAsync(int userId, int noteId);
    Task<ApiResponse<NoteResponseDto>> ToggleArchiveAsync(int userId, int noteId);
    Task<ApiResponse<NoteResponseDto>> TogglePinAsync(int userId, int noteId, NotePinDto pinDto);
}
