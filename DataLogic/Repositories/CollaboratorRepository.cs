using DataLogic.Data;
using DataLogic.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Entities;

namespace DataLogic.Repositories;

public class CollaboratorRepository(ApplicationDbContext dbContext) : ICollaboratorRepository
{
    public async Task<Collaborator> AddCollaboratorAsync(Collaborator collaborator)
    {
        dbContext.Collaborators.Add(collaborator);
        await dbContext.SaveChangesAsync();
        return collaborator;
    }

    public async Task<IEnumerable<Collaborator>> GetCollaboratorsByNoteIdAsync(int noteId, int userId)
    {
        // Ensure the note belongs to the user before showing collaborators
        var noteExists = await dbContext.Notes.AnyAsync(n => n.NoteId == noteId && n.UserId == userId);
        if (!noteExists) return Enumerable.Empty<Collaborator>();

        return await dbContext.Collaborators
            .Where(c => c.NoteId == noteId)
            .ToListAsync();
    }

    public async Task<bool> RemoveCollaboratorAsync(int collaboratorId, int userId)
    {
        // Find collaborator AND verify that the note it belongs to is owned by the user
        var collaborator = await dbContext.Collaborators
            .Include(c => c.Note)
            .FirstOrDefaultAsync(c => c.CollaboratorId == collaboratorId && c.Note.UserId == userId);
            
        if (collaborator == null) return false;

        dbContext.Collaborators.Remove(collaborator);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsCollaboratorExistsAsync(int noteId, string email)
    {
        return await dbContext.Collaborators
            .AnyAsync(c => c.NoteId == noteId && c.Email.ToLower() == email.ToLower());
    }
}
