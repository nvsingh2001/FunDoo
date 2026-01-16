using System.ComponentModel.DataAnnotations;

namespace ModelLayer.DTOs;

public class LabelRequestDto
{
    [Required(ErrorMessage = "Label Name is required")]
    public string LabelName { get; set; }
    
    public int? NoteId { get; set; }
}