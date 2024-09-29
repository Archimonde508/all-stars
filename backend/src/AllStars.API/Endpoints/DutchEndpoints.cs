using AllStars.API.DTO.Dutch;
using AllStars.Domain.Dutch.Interfaces;
using AllStars.Domain.Dutch.Models.Commands;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Serilog.ILogger;

namespace AllStars.API.Endpoints;

public static class DutchEndpoints
{
    static readonly ILogger _logger = Log.ForContext(typeof(DutchEndpoints));

    public static async Task<IResult> PutScore(
        PutScoreRequest request,
        [FromServices] IDutchService dutchService,
        [FromServices] ILoggerFactory loggerFactory,
        [FromServices] IValidator<PutScoreRequest> validator,
        CancellationToken token)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, token);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            var result = await dutchService.UpdateOne(request.GameId, request.NickName, request.Points, token);
            return result ? Results.Ok() : Results.NotFound("Something went wrong when updating DutchScore.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Something went wrong when using PutScore endpoint.");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> PostGame(
        CreateDutchGameRequest request, 
        [FromServices] IMapper mapper,
        [FromServices] IDutchService dutchService,
        [FromServices] ILoggerFactory loggerFactory,
        [FromServices] IValidator<CreateDutchGameRequest> validator,
        CancellationToken token)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, token);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            var command = mapper.Map<CreateDutchGameCommand>(request);

            await dutchService.CreateMany(command, token);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Something went wrong when using PostGame endpoint.");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> GetUserScores(
        [AsParameters] GetUserScoresQuery query,
        [FromServices] IMapper mapper,
        [FromServices] IDutchService dutchService,
        [FromServices] ILoggerFactory loggerFactory, 
        CancellationToken token)
    {
        try
        {
            var results = await dutchService.GetUserScores(query.NickName, token);
            if (results is null)
            {
                return Results.NotFound();
            }

            var response = mapper.Map<List<DutchScoreResponse>>(results);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Something went wrong when using GetUserScore endpoint.");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> GetAll(
        [FromServices] IMapper mapper,
        [FromServices] IDutchService dutchService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken token)
    {
        try
        {
            var results = await dutchService.GetAll(token);
            var response = mapper.Map<List<DutchScoreResponse>>(results);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Something went wrong when using GetAll endpoint.");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
