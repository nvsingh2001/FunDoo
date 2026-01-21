using BusinessLogicLayer.Interfaces;
using MassTransit;
using ModelLayer.Event;

namespace BusinessLogicLayer.Consumers;

public class ReminderConsumer(IEmailService emailService): IConsumer<ISendReminderEvent>
{
    public async Task Consume(ConsumeContext<ISendReminderEvent> context)
    {
        var message = $"Reminder for your note: {context.Message.NoteTitle}" +
                      $"\n\n{context.Message.NoteDescription}";

        await emailService.SendEmailAsync(
            context.Message.ToEmail,
            "FunDoo Note Reminder",
            message
        );
    }
}