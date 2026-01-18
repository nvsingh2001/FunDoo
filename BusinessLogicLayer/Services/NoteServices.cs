using AutoMapper;
using BusinessLogicLayer.Interfaces;
using DataLogic.Interfaces;
using ModelLayer.DTOs;
using ModelLayer.Entities;

namespace BusinessLogicLayer.Services;

public class NoteServices(INoteRepository noteRepository, IMapper mapper): INoteService
{
    public async Task<NoteResponseDto> CreateNoteAsync(int userId, NoteCreateDto noteCreateDto)
    {
        var note = mapper.Map<Note>(noteCreateDto);

        note.UserId = userId;
        note.CreatedAt = DateTime.UtcNow;
        note.ChangedAt = DateTime.UtcNow;
        note.IsTrash = false;
        
        var  result = await noteRepository.CreateNoteAsync(note);
        return mapper.Map<NoteResponseDto>(result);
    }
    
    public async Task<IEnumerable<NoteResponseDto>> GetAllNotesAsync(int userId, bool? isArchived = null, bool? isTrashed = null)
    {
        var notes = await noteRepository.GetAllNotesAsync(userId, isArchived, isTrashed);
        return mapper.Map<IEnumerable<NoteResponseDto>>(notes);
    }

    public async Task<NoteDetailsDto> GetNoteByIdAsync(int userId, int noteId)
    {
        var note = await  noteRepository.GetNoteByIdAsync(userId, noteId);
        
        if (note is null)
            throw new KeyNotFoundException("Note not found");
        
        return mapper.Map<NoteDetailsDto>(note);
    }

    public async Task<NoteResponseDto> UpdateNoteAsync(int userId, int noteId, NoteUpdateDto noteUpdateDto)
    {
        var existingNote = await  noteRepository.GetNoteByIdAsync(userId, noteId);
        
        if(existingNote is null)
            throw new KeyNotFoundException("Note not found");
        
        mapper.Map(noteUpdateDto, existingNote);
        existingNote.ChangedAt = DateTime.UtcNow;
        
        var result = await noteRepository.UpdateNoteAsync(existingNote);
        return mapper.Map<NoteResponseDto>(result);
    }

    public async Task DeleteNoteAsync(int userId, int noteId)
    {
        var result = await noteRepository.DeleteNoteAsync(userId, noteId);
        
        if (!result)
            throw new KeyNotFoundException("Note not found");
    }

    public async Task DeleteNotePermanentlyAsync(int userId, int noteId)
    {
        var result = await noteRepository.DeletePermanentlyAsync(userId, noteId);
        
        if (!result)
            throw new KeyNotFoundException("Note not found");
    }

    public async Task<NoteResponseDto> RestoreNoteAsync(int userId, int noteId)
    {
        var existingNote = await noteRepository.GetNoteByIdAsync(userId, noteId);
        
        if (existingNote is null)
            throw new KeyNotFoundException("Note not found");
        
        if (!existingNote.IsTrash)
            throw new InvalidOperationException("Note is not trashed");
            
        existingNote.IsTrash = false;
        existingNote.ChangedAt = DateTime.UtcNow;
        var result = await noteRepository.UpdateNoteAsync(existingNote);
        
        return mapper.Map<NoteResponseDto>(result);
    }

    public async Task<NoteResponseDto> ToggleArchiveAsync(int userId, int noteId)
    {
        var existingNote = await noteRepository.GetNoteByIdAsync(userId, noteId);
        
        if (existingNote is null)
            throw new KeyNotFoundException("Note not found");
        
        existingNote.IsArchive = !existingNote.IsArchive;
        existingNote.ChangedAt = DateTime.UtcNow;
        
        var result = await noteRepository.UpdateNoteAsync(existingNote);
        return mapper.Map<NoteResponseDto>(result);
    }

    public async Task<NoteResponseDto> TogglePinAsync(int userId, int noteId, NotePinDto pinDto)
    {
        var existingNote = await noteRepository.GetNoteByIdAsync(userId, noteId);
        
        if (existingNote is null)
            throw new KeyNotFoundException("Note not found");
        
        existingNote.IsPin = !existingNote.IsPin;
        existingNote.ChangedAt = DateTime.UtcNow;
        
        var result = await noteRepository.UpdateNoteAsync(existingNote);
        return mapper.Map<NoteResponseDto>(result);
    }

    public async Task<NoteResponseDto> UpdateColorAsync(int userId, int noteId, string color)
    {
        var existingNote = await noteRepository.GetNoteByIdAsync(userId, noteId);
        
        if (existingNote is null)
            throw new KeyNotFoundException("Note not found");
            
        existingNote.Colour = color;
        existingNote.ChangedAt = DateTime.UtcNow;
        
        var result = await noteRepository.UpdateNoteAsync(existingNote);
        return mapper.Map<NoteResponseDto>(result);
    }
}