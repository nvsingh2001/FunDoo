using System.ComponentModel.DataAnnotations;

namespace ModelLayer.DTOs;

public class CollaboratorRequestDto
{
    [EmailAddress]
    public string Email { get; set; }
}
