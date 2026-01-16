using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Entities;

public class User
{
    [Key]
    public int UserId { get; set; }
    
    [Required(ErrorMessage = "FirstName is required")]
    [MaxLength(100, ErrorMessage = "FirstName cannot exceed 100 characters")]
    [MinLength(3, ErrorMessage = "FirstName cannot less than 3 characters")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Lastname is required")]
    [MaxLength(100, ErrorMessage = "LastName cannot exceed 50 characters")]
    [MinLength(3, ErrorMessage = "LastName cannot less than 3 characters")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [MaxLength(255, ErrorMessage = "Password cannot exceed 255 characters")]
    public string Password { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    public DateTime ChangedAt { get; set; } =  DateTime.UtcNow;
    
    public ICollection<Note> Notes { get; set; }
    public ICollection<Label> Labels { get; set; }
    public ICollection<Collaborator>  Collaborators { get; set; }
}