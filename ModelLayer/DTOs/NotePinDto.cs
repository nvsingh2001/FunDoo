using System.ComponentModel.DataAnnotations;

namespace ModelLayer.DTOs;

public class NotePinDto
{
    [Required]
    public bool IsPin { get; set; }
}