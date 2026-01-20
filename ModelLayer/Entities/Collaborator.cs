using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Entities;

public class Collaborator
{
    [Key]
    public int CollaboratorId { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int NoteId { get; set; }
    public Note Note { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}