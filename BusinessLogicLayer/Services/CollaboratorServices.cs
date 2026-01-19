using AutoMapper;
using BusinessLogicLayer.Interfaces;
using DataLogic.Interfaces;
using ModelLayer.DTOs;
using ModelLayer.Entities;

namespace BusinessLogicLayer.Services;

public class CollaboratorServices(
    ICollaboratorRepository collaboratorRepository, 
    INoteRepository noteRepository, 
    IUserRepository userRepository,
    IMapper mapper) : ICollaboratorService
{
    public async Task<CollaboratorResponseDto> AddCollaboratorAsync(int userId, int noteId, CollaboratorRequestDto collaboratorDto)
    {
        var noteExists = await noteRepository.NoteExistsAsync(noteId, userId);
        if (!noteExists)
            throw new KeyNotFoundException("Note not found or you are not the owner");

        var collaboratorUser = await userRepository.GetUserByEmailAsync(collaboratorDto.Email);
        if (collaboratorUser == null)
            throw new InvalidOperationException("User with this email does not exist");

        if (collaboratorUser.UserId == userId)
            throw new InvalidOperationException("You cannot add yourself as a collaborator");

        var exists = await collaboratorRepository.IsCollaboratorExistsAsync(noteId, collaboratorDto.Email);
        if (exists)
            throw new InvalidOperationException("Collaborator already added");

        var collaborator = new Collaborator
        {
            NoteId = noteId,
            UserId = collaboratorUser.UserId, 
            Email = collaboratorDto.Email
        };

        var createdCollaborator = await collaboratorRepository.AddCollaboratorAsync(collaborator);
        return mapper.Map<CollaboratorResponseDto>(createdCollaborator);
    }

    public async Task<IEnumerable<CollaboratorResponseDto>> GetCollaboratorsAsync(int userId, int noteId)
    {
        var noteExists = await noteRepository.NoteExistsAsync(noteId, userId);
        if (!noteExists)
            throw new KeyNotFoundException("Note not found");

        var collaborators = await collaboratorRepository.GetCollaboratorsByNoteIdAsync(noteId, userId);
        return mapper.Map<IEnumerable<CollaboratorResponseDto>>(collaborators);
    }

    public async Task RemoveCollaboratorAsync(int userId, int noteId, int collaboratorId)
    {
        var noteExists = await noteRepository.NoteExistsAsync(noteId, userId);
        if (!noteExists)
            throw new KeyNotFoundException("Note not found");

        var result = await collaboratorRepository.RemoveCollaboratorAsync(collaboratorId, userId);
        
        if (!result)
            throw new KeyNotFoundException("Collaborator not found or does not belong to this note");
    }
}