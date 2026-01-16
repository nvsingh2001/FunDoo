using System.ComponentModel.DataAnnotations;

namespace ModelLayer.DTOs;

public class UserResponseDto
{
    public int UserId { get; set; }
    public string Email { get; set; }
}