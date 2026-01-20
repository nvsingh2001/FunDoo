using BusinessLogicLayer.Interfaces;
using MassTransit;
using ModelLayer.Event;

namespace BusinessLogicLayer.Consumers;

public class EmailConsumer(IEmailService emailService): IConsumer<ISendEmailEvent>
{
    public async Task Consume(ConsumeContext<ISendEmailEvent> context)
    {
        await emailService.SendEmailAsync(
            context.Message.ToEmail,
            context.Message.Subject,
            context.Message.Body
        );
    }
}