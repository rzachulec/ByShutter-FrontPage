using ByShutterBackend.Application.HomeConfig.Commands;
using ByShutterBackend.Application.HomeConfig.DTO;
using ByShutterBackend.Application.HomeConfig.Queries;
using ByShutterBackend.Application.HomeConfig.Queries.HomeProfile;
using ByShutterBackend.Web.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ByShutterBackend.Web.Endpoints.HomeConfig;

public static class HomeProfileEndpoints
{
    public static IEndpointRouteBuilder MapHomeProfileEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/home-profile");

        group.MapGet("/{homeProfileId:guid}", GetHomeProfileAsync)
            .Produces<HomeProfileDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        group.MapGet("/{pageSize:int}/{page:int}", GetHomeProfilesAsync)
            .Produces<IEnumerable<HomeProfileDto>>(StatusCodes.Status200OK);
        group.MapGet("/{homeProfileId:guid}/hero-section", GetHeroSectionAsync);
        group.MapGet("/{homeProfileId:guid}/about-me-section", GetAboutMeSectionAsync);
        group.MapGet("/{homeProfileId:guid}/home-images", GetHomeImagesAsync);
        
        group.MapPost("/", CreateHomeProfileAsync)
            .RequireAuthorization()
            .Produces<Guid>(StatusCodes.Status201Created);

        group.MapPut("/{homeProfileId:guid}", UpdateHomeProfileAsync)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{homeProfileId:guid}", DeleteHomeProfileAsync)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent);
        
        group.MapPut("/{homeProfileId:guid}/portfolio-tiles/{portfolioTileId:guid}", AssignPortfolioTile)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{homeProfileId:guid}/portfolio-tiles/{portfolioTileId:guid}",
                RemovePortfolioTile)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> GetHomeProfileAsync(Guid homeProfileId, IMediator mediator, CancellationToken ct)
    {
        var request = new GetHomeConfigByIdQuery(homeProfileId);

        var homeProfile = await mediator.Send(request, ct);

        return Results.Ok(homeProfile);
    }

    private static async Task<IResult> GetHomeProfilesAsync(int pageSize, int page, IMediator mediator,
        CancellationToken ct)
    {
        var profiles = await mediator.Send(new GetHomeProfilesQuery(pageSize, page), ct);

        return Results.Ok(profiles);
    }

    private static async Task<IResult> GetHeroSectionAsync(Guid homeProfileId, IMediator mediator, CancellationToken ct)
    {
        var query = new GetHeroSectionQuery(
            homeProfileId
        );
        
        var heroSection = await mediator.Send(query, ct);
        
        return Results.Ok(heroSection);
    }

    private static async Task<IResult> GetAboutMeSectionAsync(Guid homeProfileId, IMediator mediator, CancellationToken ct)
    {
        var query = new GetAboutMeSectionQuery(
            homeProfileId
        );
        
        var aboutMeSection = await mediator.Send(query, ct);
        
        return Results.Ok(aboutMeSection);
    }

    private static async Task<IResult> GetHomeImagesAsync(Guid homeProfileId, IMediator mediator, CancellationToken ct)
    {
        var query = new GetHomeImagesQuery(
            homeProfileId
        );
        
        var homeImages = await mediator.Send(query, ct);
        
        return Results.Ok(homeImages);
    }

    private static async Task<IResult> CreateHomeProfileAsync(CreateHomeProfileRequest request, IMediator mediator,
        CancellationToken ct)
    {
        var command = new CreateHomeProfileCommand(
            request.HeroMessage,
            request.Greetings,
            request.Quote,
            request.AboutMe,
            request.HeroImageId,
            request.ProfileImageId
        );

        var result = await mediator.Send(command, ct);

        return Results.Created($"/api/home-profile/{result.Value}", result.Value);
    }

    private static async Task<IResult> UpdateHomeProfileAsync(Guid homeProfileId,
        [FromBody] UpdateHomeProfileRequest request, IMediator mediator, CancellationToken ct)
    {
        var command = new UpdateHomeProfileCommand(
            homeProfileId,
            request.HeroMessage,
            request.Greetings,
            request.Quote,
            request.AboutMe,
            request.HeroImageId,
            request.ProfileImageId
        );

        var result = await mediator.Send(command, ct);

        return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.Errors);
    }

    private static async Task<IResult> DeleteHomeProfileAsync(Guid homeProfileId, IMediator mediator,
        CancellationToken ct)
    {
        await mediator.Send(new DeleteHomeProfileCommand(homeProfileId), ct);

        return Results.NoContent();
    }

    private static async Task<IResult> AssignPortfolioTile(Guid homeProfileId, Guid portfolioTileId, IMediator mediator,
        CancellationToken ct)
    {
        var command = new AssignPortfolioTileToProfileCommand(
            portfolioTileId,
            homeProfileId
        );
        
        var result = await mediator.Send(command, ct);

        return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.Errors);
    }

    private static async Task<IResult> RemovePortfolioTile(Guid homeProfileId, Guid portfolioTileId,
        IMediator mediator, CancellationToken ct)
    {
        var command = new RemovePortfolioTileFromProfileCommand(
            homeProfileId,
            portfolioTileId
        );

        var result = await mediator.Send(command, ct);

        return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.Errors);
    }
}