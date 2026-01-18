using AutoMapper;
using BusinessLogicLayer.Interfaces;
using DataLogic.Interfaces;
using ModelLayer.DTOs;
using ModelLayer.Entities;

namespace BusinessLogicLayer.Services;

public class LabelServices(ILabelRepository labelRepository, INoteRepository noteRepository, IMapper mapper) : ILabelService
{
    public async Task<LabelResponseDto> CreateLabelAsync(int userId, LabelRequestDto labelDto)
    {
        var label = mapper.Map<Label>(labelDto);
        label.UserId = userId;
        label.CreatedAt = DateTime.UtcNow;
        label.ChangedAt = DateTime.UtcNow;
        label.Notes = new List<Note>();

        var createdLabel = await labelRepository.CreateLabelAsync(label);

        // 2. If a NoteId was provided, establish the relationship now that both exist
        if (labelDto.NoteId.HasValue)
        {
            // Use the dedicated repository method to handle the many-to-many link
            await noteRepository.AddLabelToNoteAsync(labelDto.NoteId.Value, createdLabel);
        }

        return mapper.Map<LabelResponseDto>(createdLabel);
    }

    public async Task<IEnumerable<LabelResponseDto>> GetAllLabelsAsync(int userId)
    {
        var labels = await labelRepository.GetLabelsByUserIdAsync(userId);
        return mapper.Map<IEnumerable<LabelResponseDto>>(labels);
    }

    public async Task<IEnumerable<NoteResponseDto>> GetNotesByLabelAsync(int userId, int labelId)
    {
        var exists = await labelRepository.LabelExistsAsync(labelId, userId);
        if (!exists)
            throw new KeyNotFoundException("Label not found");

        var notes = await labelRepository.GetNotesByLabelIdAsync(labelId, userId);
        return mapper.Map<IEnumerable<NoteResponseDto>>(notes);
    }

    public async Task<LabelResponseDto> UpdateLabelAsync(int userId, int labelId, LabelRequestDto labelDto)
    {
        var existingLabel = await labelRepository.LabelExistsAsync(labelId, userId);
        if (!existingLabel)
            throw new KeyNotFoundException("Label not found");

        var labelToUpdate = new Label
        {
            LabelId = labelId,
            UserId = userId,
            LabelName = labelDto.LabelName,
            ChangedAt = DateTime.UtcNow
        };

        var result = await labelRepository.UpdateLabelAsync(labelToUpdate);
        return mapper.Map<LabelResponseDto>(result);
    }

    public async Task DeleteLabelAsync(int userId, int labelId)
    {
        var result = await labelRepository.DeleteLabelAsync(labelId, userId);
        if (!result)
            throw new KeyNotFoundException("Label not found");
    }
}
