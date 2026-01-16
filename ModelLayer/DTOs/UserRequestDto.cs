using System.ComponentModel.DataAnnotations;

namespace ModelLayer.DTOs;

public class UserRequestDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}