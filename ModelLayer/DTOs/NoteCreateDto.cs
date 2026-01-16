using System.ComponentModel.DataAnnotations;

namespace ModelLayer.DTOs;

public class NoteCreateDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public string Colour { get; set; } = "#FFFFFF";
    
    public DateTime? Reminder { get; set; }
}
