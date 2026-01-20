using DataLogic.Data;
using DataLogic.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Entities;

namespace DataLogic.Repositories;

public class NoteRepository(ApplicationDbContext dbContext) : INoteRepository
{
    public async Task<Note> CreateNoteAsync(Note note)
    {
        dbContext.Notes.Add(note);
        await dbContext.SaveChangesAsync();
        return note;
    }

    public async Task<IEnumerable<Note>> GetAllNotesAsync(int userId, bool? isArchived = null, bool? isTrash = null)
    {
        var query = dbContext.Notes.AsQueryable()
            .Where(n => n.UserId == userId || n.Collaborators.Any(c => c.UserId == userId));

        if (isArchived.HasValue)
            query = query.Where(n => n.IsArchive == isArchived.Value);
        
        if (isTrash.HasValue)
            query = query.Where(n => n.IsTrash == isTrash.Value);

        return await query
            .Include(n => n.Labels)
            .Include(n => n.Collaborators)
            .AsSplitQuery() 
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Note?> GetNoteByIdAsync(int noteId, int userId)
    {
        return await dbContext.Notes
            .Include(n => n.Labels)
            .Include(n => n.Collaborators)
            .AsSplitQuery() 
            .FirstOrDefaultAsync(n => n.NoteId == noteId && (n.UserId == userId || n.Collaborators.Any(c => c.UserId == userId)));
    }

    public async Task<Note> UpdateNoteAsync(Note note)
    {
        var existingNote = await dbContext.Notes.FirstOrDefaultAsync(n => n.NoteId == note.NoteId && n.UserId == note.UserId);
        if (existingNote == null) return null;

        existingNote.Title = note.Title;
        existingNote.Description = note.Description;
        existingNote.Reminder = note.Reminder;
        existingNote.Colour = note.Colour;
        existingNote.Image = note.Image;
        existingNote.IsArchive = note.IsArchive;
        existingNote.IsPin = note.IsPin;
        existingNote.IsTrash = note.IsTrash;
        existingNote.ChangedAt = DateTime.UtcNow;

        dbContext.Notes.Update(existingNote);
        await dbContext.SaveChangesAsync();
        return existingNote;
    }

    public async Task<bool> DeleteNoteAsync(int noteId, int userId)
    {
        var note = await dbContext.Notes.FirstOrDefaultAsync(n => n.NoteId == noteId && n.UserId == userId);
        if (note == null) return false;

        note.IsTrash = true;
        note.ChangedAt = DateTime.UtcNow;
        
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePermanentlyAsync(int noteId, int userId)
    {
        var note = await dbContext.Notes.FirstOrDefaultAsync(n => n.NoteId == noteId && n.UserId == userId);
        if (note == null) return false;

        dbContext.Notes.Remove(note);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> NoteExistsAsync(int noteId, int userId)
    {
        return await dbContext.Notes.AnyAsync(n => n.NoteId == noteId && (n.UserId == userId || n.Collaborators.Any(c => c.UserId == userId)));
    }

    public async Task AddLabelToNoteAsync(int noteId, Label label)
    {
        var note = await dbContext.Notes
            .Include(n => n.Labels)
            .FirstOrDefaultAsync(n => n.NoteId == noteId);

        if (note != null)
        {
            note.Labels.Add(label);
            await dbContext.SaveChangesAsync();
        }
    }
}
