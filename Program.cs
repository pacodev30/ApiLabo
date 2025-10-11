using ApiLabo.Data;
using ApiLabo.Endpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApiDbContext>(opt => opt.UseSqlServer(
    builder.Configuration.GetConnectionString("SqlServer")) 
);

builder.Services.MapUserServices();

builder.Logging.ClearProviders();
var loggerConfiguration = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);
var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
    app.UseSwagger();
}

await app.Services
    .CreateScope().ServiceProvider
    .GetRequiredService<ApiDbContext>().Database
    .MigrateAsync();

app.MapGet("", () => Results.Redirect("/swagger"));

app.MapGroup("/user")
    .MapUserEndpoints();

app.Run();