using ByShutterBackend.Application.HomeConfig.Commands;
using ByShutterBackend.Application.HomeConfig.DTO;
using ByShutterBackend.Application.HomeConfig.Queries;
using ByShutterBackend.Web.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ByShutterBackend.Web.Endpoints.HomeConfig;

public static class PortfolioTilesEndpoints
{
    public static IEndpointRouteBuilder MapPortfolioTilesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/portfolio-tiles");

        group.MapGet("/{pageSize:int}/{page:int}", GetPortfolioTilesAsync)
            .Produces<IEnumerable<PortfolioTileDto>>(StatusCodes.Status200OK);
        group.MapGet("/{portfolioTileId:guid}", GetPortfolioTileAsync)
            .Produces<PortfolioTileDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        
        group.MapPost("/", CreatePortfolioTileAsync)
            .RequireAuthorization()
            .Produces<Guid>(StatusCodes.Status201Created);

        group.MapPut("/{portfolioTileId:guid}", UpdatePortfolioTileAsync)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{portfolioTileId:guid}", DeletePortfolioTileAsync)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent);

        return app;
    }

    private static async Task<IResult> GetPortfolioTileAsync(Guid portfolioTileId, IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetPortfolioTileByIdQuery(portfolioTileId), ct);

        return Results.Ok(result);
    }

    private static async Task<IResult> GetPortfolioTilesAsync(int pageSize, int page, IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetPortfolioTilesQuery(
            pageSize,
            page
        );
        
        var portfolioTiles = await mediator.Send(query, ct);
        
        return Results.Ok(portfolioTiles);
    }
    
    private static async Task<IResult> CreatePortfolioTileAsync(CreatePortfolioTileRequest request, IMediator mediator,
        CancellationToken ct)
    {
        var command = new CreatePortfolioTileCommand(
            request.Name,
            request.ImageId,
            request.Description
        );
        
        var result = await mediator.Send(command, ct);

        return Results.Created($"/api/portfolio-tiles/{result.Value}", result.Value);
    }

    private static async Task<IResult> UpdatePortfolioTileAsync(Guid portfolioTileId,
        [FromBody] UpdatePortfolioTileRequest request, IMediator mediator, CancellationToken ct)
    {
        var command = new UpdatePortfolioTileCommand(
            portfolioTileId,
            request.Name,
            request.ImageId,
            request.Description
        );

        await mediator.Send(command, ct);

        return Results.NoContent();
    }

    private static async Task<IResult> DeletePortfolioTileAsync(Guid portfolioTileId, IMediator mediator,
        CancellationToken ct)
    {
        await mediator.Send(new DeletePortfolioTileCommand(portfolioTileId), ct);

        return Results.NoContent();
    }
}