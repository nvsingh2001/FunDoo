using ModelLayer.Entities;

namespace DataLogic.Interfaces;

public interface INoteRepository
{
    Task<Note> CreateNoteAsync(Note note);
    Task<IEnumerable<Note>> GetAllNotesAsync(int userId, bool? isArchived = null, bool? isTrash = null);
    Task<Note> GetNoteByIdAsync(int noteId, int userId);
    Task<Note> UpdateNoteAsync(Note note);
    Task<bool> DeleteNoteAsync(int noteId, int userId); // Typically soft delete
    Task<bool> NoteExistsAsync(int noteId, int userId);
}
