using AutoMapper;
using ModelLayer.DTOs;
using ModelLayer.Entities;

namespace BusinessLogicLayer.Mapping;

public class UserProfile: Profile 
{
   public UserProfile()
   {
      CreateMap<User, UserResponseDto>().ReverseMap();
   } 
}