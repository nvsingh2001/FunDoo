using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using BusinessLogicLayer.Services;
using DataLogic.Data;
using DataLogic.Interfaces;
using DataLogic.Repositories;
using FunDooApp.Extensions;
using FunDooApp.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddJwtAuthentication();

builder.Services.AddControllers();

builder.Services.AddAuthorization();

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.CommandTimeout(30))
);

builder.Services.AddSingleton<ITokenService, TokenServices>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<INoteRepository, NoteRepository>();

builder.Services.AddScoped<ILabelRepository, LabelRepository>();

builder.Services.AddScoped<IUserService, UserServices>();

builder.Services.AddScoped<INoteService, NoteServices>();

builder.Services.AddScoped<ILabelService, LabelServices>();

builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.AddAutoMapper(typeof(NoteProfile));

builder.Services.AddAutoMapper(typeof(LabelProfile));

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.Run();
