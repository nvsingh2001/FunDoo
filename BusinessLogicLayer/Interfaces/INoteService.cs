using ModelLayer.DTOs;
using ModelLayer.Utilities;

namespace BusinessLogicLayer.Interfaces;

public interface INoteService
{
    Task<NoteResponseDto> CreateNoteAsync(int userId, NoteCreateDto noteDto);
    Task<IEnumerable<NoteResponseDto>> GetAllNotesAsync(int userId, bool? isArchived = null, bool? isTrashed = null);
    Task<NoteDetailsDto> GetNoteByIdAsync(int userId, int noteId);
    Task<NoteResponseDto> UpdateNoteAsync(int userId, int noteId, NoteUpdateDto noteDto);
    Task DeleteNoteAsync(int userId, int noteId);
    Task DeleteNotePermanentlyAsync(int userId, int noteId);
    Task<NoteResponseDto> RestoreNoteAsync(int userId, int noteId);
    Task<NoteResponseDto> ToggleArchiveAsync(int userId, int noteId);
    Task<NoteResponseDto> TogglePinAsync(int userId, int noteId, NotePinDto pinDto);
    Task<NoteResponseDto> UpdateColorAsync(int userId, int noteId, string color);
    Task ProcessDueRemindersAsync();
}
