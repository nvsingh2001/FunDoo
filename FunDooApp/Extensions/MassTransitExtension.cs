using BusinessLogicLayer.Consumers;
using MassTransit;

namespace FunDooApp.Extensions;

public static class MassTransitExtension
{
    public static void AddConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(config =>
        {
            config.AddConsumer<EmailConsumer>();
            config.AddConsumer<ReminderConsumer>();

            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                    {
                        h.Username(configuration.GetValue<string>("RabbitMqSettings:Username")!);
                        h.Password(configuration.GetValue<string>("RabbitMqSettings:Password")!);
                    }
                );

                cfg.ReceiveEndpoint("email-queue", e =>
                    {
                        e.ConfigureConsumer<EmailConsumer>(ctx);
                    }
                );

                cfg.ReceiveEndpoint("reminder-queue", e =>
                    {
                        e.UseRateLimit(1, TimeSpan.FromSeconds(2));
                        e.ConfigureConsumer<ReminderConsumer>(ctx);
                    }
                );
            });
        });
    }
}