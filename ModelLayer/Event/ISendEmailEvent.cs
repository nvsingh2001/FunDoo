namespace ModelLayer.Event;

public interface ISendEmailEvent
{
    string ToEmail { get; }
    string Subject { get; }
    string Body { get; }
}