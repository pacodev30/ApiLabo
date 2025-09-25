using ApiLabo.Data;
using ApiLabo.Data.Models;
using ApiLabo.Dto;
using ApiLabo.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiDbContext>(opt => opt.UseSqlServer(
    builder.Configuration.GetConnectionString("SqlServer")) 
);

builder.Services.AddScoped<IUserService, EFCoreUserService>();

builder.Logging.ClearProviders();
var loggerConfiguration = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);
var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

await app.Services
    .CreateScope().ServiceProvider
    .GetRequiredService<ApiDbContext>().Database
    //.EnsureCreated();
    .MigrateAsync();

// -- ENDPOINTS
app.MapGet("/users", async (
    [FromServices] IUserService service,
    [FromServices] ILogger<Program> logger) =>
    {
        var users = await service.GetAll();
        logger.LogInformation("Get {count} users", users.Count);
        return Results.Ok(users);
    });

app.MapGet("/users/{id:int}", async (
    [FromRoute] int id,
    [FromServices] IUserService service) =>
    {
        var userOutput = await service.GetById(id);
        if (userOutput is null) return Results.NotFound();
        return Results.Ok(userOutput);
    });

app.MapDelete("/users/{id:int}", async (
    [FromRoute] int id,
    [FromServices] IUserService service) =>
    {
        var result = await service.Delete(id);
        if (result) return Results.NoContent();
        return Results.NotFound();
    });

app.MapPut("/users/{id:int}", async (
    [FromRoute] int id,
    [FromBody] UserInputModel userInput,
    [FromServices] IUserService service) =>
    {
        var result = await service.Update(id, userInput);
        if (result) return Results.NoContent();
        return Results.NotFound();
    });

app.MapPost("/users", async (
    [FromBody] UserInputModel newUserInput,
    [FromServices] IValidator<UserInputModel> validator,
    [FromServices] IUserService service) =>
    {
        var result = validator.Validate(newUserInput);
        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => new
            {
                e.ErrorMessage,
                e.PropertyName
            });

            return Results.BadRequest(errors);
        }
        await service.Add(newUserInput);
        return Results.Ok(newUserInput);
    });

app.Run();