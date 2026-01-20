namespace ModelLayer.DTOs;

public class NoteDetailsDto
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
    public DateTime? ChangedAt { get; set; }
    
    public ICollection<LabelResponseDto> Labels { get; set; }
    public ICollection<CollaboratorResponseDto> Collaborators { get; set; }
}