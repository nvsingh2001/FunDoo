using BusinessLogicLayer.Interfaces;

namespace FunDooApp.BackgroundService;

public class NoteReminderService(IServiceProvider serviceProvider): Microsoft.Extensions.Hosting.BackgroundService
{
    protected override async Task  ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var noteService = scope.ServiceProvider.GetRequiredService<INoteService>();

                await noteService.ProcessDueRemindersAsync();
            }
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}