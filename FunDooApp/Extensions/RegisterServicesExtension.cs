using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using DataLogic.Interfaces;
using DataLogic.Repositories;
using FunDooApp.BackgroundService;

namespace FunDooApp.Extensions;

public static class RegisterServicesExtension
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddHostedService<NoteReminderService>();
        
        services.AddSingleton<ITokenService, TokenServices>();

        services.AddTransient<IEmailService, EmailServices>();

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<INoteRepository, NoteRepository>();

        services.AddScoped<ILabelRepository, LabelRepository>();

        services.AddScoped<ICollaboratorRepository, CollaboratorRepository>();

        services.AddScoped<IUserService, UserServices>();

        services.AddScoped<INoteService, NoteServices>();

        services.AddScoped<ILabelService, LabelServices>();

        services.AddScoped<ICollaboratorService, CollaboratorServices>();
    }
}