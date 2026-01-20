using AutoMapper;
using BusinessLogicLayer.Extensions;
using BusinessLogicLayer.Interfaces;
using DataLogic.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using ModelLayer.DTOs;
using ModelLayer.Entities;

namespace BusinessLogicLayer.Services;

public class LabelServices(ILabelRepository labelRepository, INoteRepository noteRepository,IDistributedCache distributedCache,  IMapper mapper) : ILabelService
{
    private async Task ClearLabelNoteCacheAsync(int userId, int labelId, IEnumerable<int>? noteIds = null)
    {
        await distributedCache.RemoveAsync($"labels_{userId}");
        await distributedCache.RemoveAsync($"notes_{userId}_{labelId}");

        if (noteIds != null)
        {
            foreach (var nid in noteIds)
            {
                await distributedCache.RemoveAsync($"notes_{userId}_{nid}");
            }

            await distributedCache.RemoveAsync($"notes_{userId}_{true}_{false}");
            await distributedCache.RemoveAsync($"notes_{userId}_{false}_{true}");
            await distributedCache.RemoveAsync($"notes_{userId}_{false}_{false}");
            await distributedCache.RemoveAsync($"notes_{userId}_{null}_{null}");
        }
    }
    
    public async Task<LabelResponseDto> CreateLabelAsync(int userId, LabelRequestDto labelDto)
    {
        var label = mapper.Map<Label>(labelDto);
        label.UserId = userId;
        label.CreatedAt = DateTime.UtcNow;
        label.ChangedAt = DateTime.UtcNow;
        label.Notes = new List<Note>();

        var createdLabel = await labelRepository.CreateLabelAsync(label);

        List<int>? affectedNoteIds = null;
        if (labelDto.NoteId.HasValue)
        {
            await noteRepository.AddLabelToNoteAsync(labelDto.NoteId.Value, createdLabel);
            affectedNoteIds = new List<int> { labelDto.NoteId.Value };
        }
        
        await ClearLabelNoteCacheAsync(userId, createdLabel.LabelId, affectedNoteIds);

        return mapper.Map<LabelResponseDto>(createdLabel);
    }

    public async Task<IEnumerable<LabelResponseDto>> GetAllLabelsAsync(int userId)
    {
        var cacheKey = $"labels_{userId}";
        
        var cacheOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20))
            .SetSlidingExpiration(TimeSpan.FromMinutes(2));
        
        var labels = await distributedCache.GetOrSetAsync(cacheKey,
            async () => mapper.Map<IEnumerable<LabelResponseDto>>(
                await labelRepository.GetLabelsByUserIdAsync(userId)
            ),
            cacheOptions
        );
        return labels!;
    }

    public async Task<IEnumerable<NoteResponseDto>> GetNotesByLabelAsync(int userId, int labelId)
    {
        var cacheKey = $"notes_{userId}_{labelId}";
        var cacheOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20))
            .SetSlidingExpiration(TimeSpan.FromMinutes(2));
        
        var exists = await labelRepository.LabelExistsAsync(labelId, userId);
        if (!exists)
            throw new KeyNotFoundException("Label not found");

        var notes = await distributedCache.GetOrSetAsync(cacheKey,
            async () => mapper.Map<IEnumerable<NoteResponseDto>>(
                await labelRepository.GetNotesByLabelIdAsync(labelId, userId)
            ),
            cacheOptions
        );
        
        return notes!;
    }

    public async Task<LabelResponseDto> UpdateLabelAsync(int userId, int labelId, LabelRequestDto labelDto)
    {
        var existingLabel = await labelRepository.LabelExistsAsync(labelId, userId);
        if (!existingLabel)
            throw new KeyNotFoundException("Label not found");

        var affectedNotes = await labelRepository.GetNotesByLabelIdAsync(labelId, userId);
        var affectedNoteIds = affectedNotes.Select(n => n.NoteId).ToList();

        var labelToUpdate = new Label
        {
            LabelId = labelId,
            UserId = userId,
            LabelName = labelDto.LabelName,
            ChangedAt = DateTime.UtcNow
        };

        var result = await labelRepository.UpdateLabelAsync(labelToUpdate);
        
        await ClearLabelNoteCacheAsync(userId, labelId, affectedNoteIds);
        
        return mapper.Map<LabelResponseDto>(result);
    }

    public async Task DeleteLabelAsync(int userId, int labelId)
    {
        var affectedNotes = await labelRepository.GetNotesByLabelIdAsync(labelId, userId);
        var affectedNoteIds = affectedNotes.Select(n => n.NoteId).ToList();

        var result = await labelRepository.DeleteLabelAsync(labelId, userId);
        
        if (!result)
            throw new KeyNotFoundException("Label not found");
        
        await ClearLabelNoteCacheAsync(userId, labelId, affectedNoteIds);
    }
}
