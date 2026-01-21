namespace ModelLayer.Event;

public interface ISendReminderEvent
{
    string ToEmail { get; set; }
    string NoteTitle { get; set; }
    string NoteDescription { get; set; }
}