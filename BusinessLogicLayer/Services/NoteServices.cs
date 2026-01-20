using AutoMapper;
using BusinessLogicLayer.Extensions;
using BusinessLogicLayer.Interfaces;
using DataLogic.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using ModelLayer.DTOs;
using ModelLayer.Entities;

namespace BusinessLogicLayer.Services;

public class NoteServices(INoteRepository noteRepository, IMapper mapper, IDistributedCache distributedCache): INoteService
{
    private async Task ClearNoteCacheAsync(int userId, int? noteId, IEnumerable<int>? labelIds = null)
    {
        await distributedCache.RemoveAsync($"notes_{userId}_{true}_{false}");
        await distributedCache.RemoveAsync($"notes_{userId}_{false}_{true}");
        await distributedCache.RemoveAsync($"notes_{userId}_{false}_{false}");
        await distributedCache.RemoveAsync($"notes_{userId}_{null}_{null}");
        await distributedCache.RemoveAsync($"notes_{userId}");
        
        if(noteId is not null)
            await distributedCache.RemoveAsync($"notes_{userId}_{noteId}");
            
        if (labelIds != null)
        {
            foreach (var labelId in labelIds)
            {
                await distributedCache.RemoveAsync($"notes_{userId}_{labelId}");
            }
        }
    }
    public async Task<NoteResponseDto> CreateNoteAsync(int userId, NoteCreateDto noteCreateDto)
    {
        var note = mapper.Map<Note>(noteCreateDto);

        note.UserId = userId;
        note.CreatedAt = DateTime.UtcNow;
        note.ChangedAt = DateTime.UtcNow;
        note.IsTrash = false;
        
        var result = await noteRepository.CreateNoteAsync(note);

        await ClearNoteCacheAsync(userId, null);
        
        return mapper.Map<NoteResponseDto>(result);
    }
    
    public async Task<IEnumerable<NoteResponseDto>> GetAllNotesAsync(int userId, bool? isArchived = null, bool? isTrashed = null)
    {
        var cacheKey = $"notes_{userId}_{isArchived}_{isTrashed}";
        var cacheOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20))
            .SetSlidingExpiration(TimeSpan.FromMinutes(2));

        var notes = await distributedCache.GetOrSetAsync(cacheKey,
            async () => mapper.Map<IEnumerable<NoteResponseDto>>(
                await noteRepository.GetAllNotesAsync(userId, isArchived, isTrashed)
            ),
            cacheOptions
        );
        
        return notes!;
    }

    public async Task<NoteDetailsDto> GetNoteByIdAsync(int userId, int noteId)
    {
        var cacheKey =  $"notes_{userId}_{noteId}";
        var note = await distributedCache.GetOrSetAsync(cacheKey,
            async () =>mapper.Map<NoteDetailsDto>(await noteRepository.GetNoteByIdAsync(userId, noteId))
        );  
        
        if (note is null)
            throw new KeyNotFoundException("Note not found");
        
        return note;
    }

    public async Task<NoteResponseDto> UpdateNoteAsync(int userId, int noteId, NoteUpdateDto noteUpdateDto)
    {
        var existingNote = await  noteRepository.GetNoteByIdAsync(userId, noteId);
        
        if(existingNote is null)
            throw new KeyNotFoundException("Note not found");
        
        mapper.Map(noteUpdateDto, existingNote);
        existingNote.ChangedAt = DateTime.UtcNow;
        
        var result = await noteRepository.UpdateNoteAsync(existingNote);
        
        await ClearNoteCacheAsync(userId, noteId, existingNote.Labels?.Select(l => l.LabelId));
        
        return mapper.Map<NoteResponseDto>(result);
    }

    public async Task DeleteNoteAsync(int userId, int noteId)
    {
        // Fetch note first to get labels for cache clearing
        var existingNote = await noteRepository.GetNoteByIdAsync(noteId, userId);
        var result = await noteRepository.DeleteNoteAsync(noteId, userId); // Fixed param order: noteId, userId
        
        if (!result)
            throw new KeyNotFoundException("Note not found");
        
        await ClearNoteCacheAsync(userId, noteId, existingNote?.Labels?.Select(l => l.LabelId));
    }

    public async Task DeleteNotePermanentlyAsync(int userId, int noteId)
    {
        var existingNote = await noteRepository.GetNoteByIdAsync(noteId, userId);
        var result = await noteRepository.DeletePermanentlyAsync(noteId, userId); // Fixed param order: noteId, userId
        
        if (!result)
            throw new KeyNotFoundException("Note not found");
        
        await ClearNoteCacheAsync(userId, noteId, existingNote?.Labels?.Select(l => l.LabelId));
    }

    public async Task<NoteResponseDto> RestoreNoteAsync(int userId, int noteId)
    {
        var existingNote = await noteRepository.GetNoteByIdAsync(noteId, userId); // Fixed param order: noteId, userId
        
        if (existingNote is null)
            throw new KeyNotFoundException("Note not found");
        
        if (!existingNote.IsTrash)
            throw new InvalidOperationException("Note is not trashed");
            
        existingNote.IsTrash = false;
        existingNote.ChangedAt = DateTime.UtcNow;
        var result = await noteRepository.UpdateNoteAsync(existingNote);
        
        await ClearNoteCacheAsync(userId, noteId, existingNote.Labels?.Select(l => l.LabelId));
        
        return mapper.Map<NoteResponseDto>(result);
    }

    public async Task<NoteResponseDto> ToggleArchiveAsync(int userId, int noteId)
    {
        var existingNote = await noteRepository.GetNoteByIdAsync(noteId, userId); // Fixed param order: noteId, userId
        
        if (existingNote is null)
            throw new KeyNotFoundException("Note not found");
        
        existingNote.IsArchive = !existingNote.IsArchive;
        existingNote.ChangedAt = DateTime.UtcNow;
        
        var result = await noteRepository.UpdateNoteAsync(existingNote);
        
        await  ClearNoteCacheAsync(userId, noteId, existingNote.Labels?.Select(l => l.LabelId)); 
        
        return mapper.Map<NoteResponseDto>(result);
    }

    public async Task<NoteResponseDto> TogglePinAsync(int userId, int noteId, NotePinDto pinDto)
    {
        var existingNote = await noteRepository.GetNoteByIdAsync(noteId, userId); // Fixed param order: noteId, userId
        
        if (existingNote is null)
            throw new KeyNotFoundException("Note not found");
        
        existingNote.IsPin = pinDto.IsPin;
        existingNote.ChangedAt = DateTime.UtcNow;
        
        var result = await noteRepository.UpdateNoteAsync(existingNote);
        
        await ClearNoteCacheAsync(userId, noteId, existingNote.Labels?.Select(l => l.LabelId));
        
        return mapper.Map<NoteResponseDto>(result);
    }

    public async Task<NoteResponseDto> UpdateColorAsync(int userId, int noteId, string color)
    {
        var existingNote = await noteRepository.GetNoteByIdAsync(noteId, userId); // Fixed param order: noteId, userId
        
        if (existingNote is null)
            throw new KeyNotFoundException("Note not found");
            
        existingNote.Colour = color;
        existingNote.ChangedAt = DateTime.UtcNow;
        
        var result = await noteRepository.UpdateNoteAsync(existingNote);
        
        await ClearNoteCacheAsync(userId, noteId, existingNote.Labels?.Select(l => l.LabelId));
        
        return mapper.Map<NoteResponseDto>(result);
    }
}