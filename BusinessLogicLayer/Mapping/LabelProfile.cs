using AutoMapper;
using ModelLayer.DTOs;
using ModelLayer.Entities;

namespace BusinessLogicLayer.Mapping;

public class LabelProfile : Profile
{
    public LabelProfile()
    {
        CreateMap<LabelRequestDto, Label>();
        CreateMap<Label, LabelResponseDto>();
    }
}
