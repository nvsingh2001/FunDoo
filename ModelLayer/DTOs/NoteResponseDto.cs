using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace ModelLayer.DTOs;

public class NoteResponseDto
{
    public int NoteId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Colour { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UserId { get; set; }
    public bool IsPin { get; set; }
    public bool IsArchive { get; set; }
    public bool IsTrash { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

