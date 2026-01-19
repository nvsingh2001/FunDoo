using AutoMapper;
using ModelLayer.DTOs;
using ModelLayer.Entities;

namespace BusinessLogicLayer.Mapping;

public class CollaboratorProfile:  Profile
{
    public CollaboratorProfile()
    {
        CreateMap<CollaboratorRequestDto, Collaborator>();
        CreateMap<Collaborator, CollaboratorResponseDto>();
    }
}