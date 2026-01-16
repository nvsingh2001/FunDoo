using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Entities;

public class Label
{
    [Key]
    public int LabelId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string LabelName { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public ICollection<Note> Notes { get; set; }
}