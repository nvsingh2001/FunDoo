using ModelLayer.Entities;

namespace DataLogic.Interfaces;

public interface ICollaboratorRepository
{
    Task<Collaborator> AddCollaboratorAsync(Collaborator collaborator);
    Task<IEnumerable<Collaborator>> GetCollaboratorsByNoteIdAsync(int noteId, int userId);
    Task<bool> RemoveCollaboratorAsync(int collaboratorId, int userId);
    Task<bool> IsCollaboratorExistsAsync(int noteId, string email);
}
