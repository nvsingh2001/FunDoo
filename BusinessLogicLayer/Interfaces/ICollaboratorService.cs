using ModelLayer.DTOs;
using ModelLayer.Utilities;

namespace BusinessLogicLayer.Interfaces;

public interface ICollaboratorService
{
    Task<ApiResponse<CollaboratorResponseDto>> AddCollaboratorAsync(int userId, int noteId, CollaboratorRequestDto collaboratorDto);
    Task<ApiResponse<IEnumerable<CollaboratorResponseDto>>> GetCollaboratorsAsync(int userId, int noteId);
    Task<ApiResponse<string>> RemoveCollaboratorAsync(int userId, int noteId, int collaboratorId);
}
