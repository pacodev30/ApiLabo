using ApiLabo.Dto;
using ApiLabo.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ApiLabo.Endpoints;

public static class UserEndpoints
{
    public static IServiceCollection MapUserServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, EFCoreUserService>();
        return services;
    }
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        // GET user
        group.MapGet("", GetAll)
            .WithTags("UserManagement")
            .WithName("GetAll")
            .Produces(200);

        // GET user/1
        group.MapGet("{id}", GetById)
            .WithTags("UserManagement")
            .WithName("GetById")
            .Produces(200)
            .Produces(404);

        // DELETE user/1
        group.MapDelete("{id}", Delete)
            .WithTags("UserManagement")
            .WithName("Delete")
            .Produces(204)
            .Produces(404);

        // PUT user/1
        group.MapPut("{id}", Update)
            .WithTags("UserManagement")
            .WithName("Update")
            .Produces(204)
            .Produces(404);

        // POST user
        group.MapPost("", Create)
            .WithTags("UserManagement")
            .WithName("Create")
            .Accepts<UserInputModel>(contentType: "application/json")
            .Produces<UserOutputModel>(contentType: "application/json")
            .Produces(200)
            .Produces(404);
        return group;
    }
    private static async Task<IResult> GetAll(
                [FromServices] IUserService service,
                [FromServices] ILogger<Program> logger)
    {
        var users = await service.GetAll();
        logger.LogInformation("Get {count} users", users.Count);
        return Results.Ok(users);
    }

    private static async Task<IResult> GetById(
            [FromRoute] string id,
            [FromServices] IUserService service)
    {
        var userOutput = await service.GetById (id);
        if (userOutput is null) return Results.NotFound();
        return Results.Ok(userOutput);
    }

    private static async Task<IResult> Delete(
        [FromRoute] string id,
        [FromServices] IUserService service)
    {
        var result = await service.Delete(id);
        if (result) return Results.NoContent();
        return Results.NotFound();
    }

    private static async Task<IResult> Update(
        [FromRoute] string id,
        [FromBody] UserInputModel userInput,
        [FromServices] IUserService service)
    {
        var result = await service.Update(id, userInput);
        if (result) return Results.NoContent();
        return Results.NotFound();
    }

    private static async Task<IResult> Create(
        [FromBody] UserInputModel newUserInput,
        [FromServices] IValidator<UserInputModel> validator,
        [FromServices] IUserService service,
        [FromServices] LinkGenerator linkGenerator,
        HttpContext httpContext)
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
        var dbResult = await service.Add(newUserInput);
        //var link = linkGenerator.GetPathByName("GetById", new { id = dbResult.Id });
        var link = linkGenerator.GetUriByName(httpContext, "GetById", new { id = dbResult.Id });
        return Results.Created(link, dbResult);
    }
};

