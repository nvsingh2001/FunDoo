using ModelLayer.DTOs;
using ModelLayer.Utilities;

namespace BusinessLogicLayer.Interfaces;

public interface ICollaboratorService
{
    Task<CollaboratorResponseDto> AddCollaboratorAsync(int userId, int noteId, CollaboratorRequestDto collaboratorDto);
    Task<IEnumerable<CollaboratorResponseDto>> GetCollaboratorsAsync(int userId, int noteId);
    Task RemoveCollaboratorAsync(int userId, int noteId, int collaboratorId);
}
