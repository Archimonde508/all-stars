using AllStars.API.DTO.Dutch;
using AllStars.API.DTO.User;
using AllStars.Application.Services.Dutch;
using AllStars.Domain.Dutch.Interfaces;
using AllStars.Domain.User.Interfaces;
using AllStars.Domain.User.Models;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Serilog.ILogger;

namespace AllStars.API.Endpoints;

public static class UserEndpoints
{
    static readonly ILogger _logger = Log.ForContext(typeof(AuthEndpoints));

    public static async Task<IResult> RegisterUser(
        CreateUserRequest request,
        [FromServices] IUserService userService,
        [FromServices] IValidator<CreateUserRequest> validator,
        [FromServices] IMapper mapper,
        CancellationToken token)
    {
        try
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            var user = mapper.Map<AllStarUser>(request);

            await userService.RegisterUserAsync(user, request.Password, token);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Something went wrong when using AddUser endpoint.");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> GetAllNickNames(
       [FromServices] IUserService userService,
       [FromServices] IMapper mapper,
       CancellationToken token)
    {
        try
        {
            var results = await userService.GetAllNicknames(token);
            var response = mapper.Map<UserNickNamesResponse>(results);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Something went wrong when using AddUser endpoint.");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> RegisterMockedUsers(
        [FromServices] IUserService userService,
        CancellationToken token)
    {
        try
        {
            // use it as tuples with password
            var users = new List<AllStarUser>()
            {
                new AllStarUser(){ FirstName = "Patryk", LastName = "Olszewski", Nickname = "Patols77" }
            };

            foreach (var user in users)
            {
                var task = userService.RegisterUserAsync(user, request.Password, token);
            }

            
            return Results.Ok();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Something went wrong when using AddUser endpoint.");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
