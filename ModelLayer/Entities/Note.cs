using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Entities;

public class Note
{
    [Key]
    public int NoteId { get; set; }
    
    [Required]
    [MaxLength(255, ErrorMessage = "The Title cannot exceed 255 characters")]
    [MinLength(3, ErrorMessage = "The Title cannot be less than 3 characters")]
    public string Title { get; set; }
    
    public string? Description { get; set; }
    public DateTime? Reminder { get; set; } 
    public string Colour { get; set; } = "#FFFFFF";
    public string? Image { get; set; }
    public bool IsArchive { get; set; } = false;
    public bool IsPin { get; set; } = false;
    public bool IsTrash { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public ICollection<Label> Labels { get; set; }
    public ICollection<Collaborator> Collaborators { get; set; }
}