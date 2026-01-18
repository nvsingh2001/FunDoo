using AutoMapper;
using ModelLayer.DTOs;
using ModelLayer.Entities;

namespace BusinessLogicLayer.Mapping;

public class NoteProfile:  Profile
{
    public NoteProfile()
    {
        CreateMap<Note, NoteResponseDto>().ReverseMap();
        
        CreateMap<NoteCreateDto, Note>();
        
        CreateMap<Note, NoteDetailsDto>().ReverseMap();
        
        CreateMap<NoteUpdateDto, Note>();
    }
}