using ByShutterBackend.Application.Portfolio.Commands;
using ByShutterBackend.Application.Portfolio.Commands.Albums;
using ByShutterBackend.Application.Portfolio.DTO;
using ByShutterBackend.Application.Portfolio.Queries.Albums;
using ByShutterBackend.Web.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ByShutterBackend.Web.Endpoints.Portfolio;

public static class AlbumEndpoints
{
    public static IEndpointRouteBuilder MapAlbumEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/albums");

        group.MapGet("/{albumId:guid}", GetAlbumAsync)
            .Produces<AlbumDto>();
        
        group.MapGet("/{albumId:guid}/header", GetAlbumHeader);
        
        group.MapGet("/{albumId:guid}/images/{pageSize:int}/{page:int}", GetAlbumImagesAsync)
            .Produces<IEnumerable<PhotoDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        
        group.MapGet("/{name}", GetAlbumByNameAsync);
        
        group.MapGet("/{pageSize:int}/{page:int}", GetAlbumsAsync);
        
        group.MapGet("/{pageSize:int}/{page:int}/metadata", GetAlbumsMetadataAsync)
            .Produces<IEnumerable<AlbumMetaDataDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/", CreateAlbumAsync).RequireAuthorization();

        group.MapDelete("/{albumId:guid}", DeleteAlbumAsync)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{albumId:guid}/add-images", AddImagesToAlbumAsync).RequireAuthorization();

        group.MapDelete("/{albumId:guid}/images", RemoveImagesFromAlbumAsync)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> GetAlbumAsync(Guid albumId, IMediator mediator, CancellationToken ct)
    {
        var query = new GetAlbumByIdQuery(
            albumId
        );

        var album = await mediator.Send(query, ct);

        return Results.Ok(album);
    }

    private static async Task<IResult> GetAlbumByNameAsync(string name, IMediator mediator, CancellationToken ct)
    {
        var query = new GetAlbumByNameQuery(
            name
        );

        var album = await mediator.Send(query, ct);

        return Results.Ok(album);
    }

    private static async Task<IResult> GetAlbumsAsync(int pageSize, int page, IMediator mediator, CancellationToken ct)
    {
        var query = new GetAlbumsQuery(
            pageSize,
            page
        );

        var albums = await mediator.Send(query, ct);

        return Results.Ok(albums);
    }

    private static async Task<IResult> GetAlbumHeader(Guid albumId, IMediator mediator, CancellationToken ct)
    {
        var query = new GetAlbumByIdQuery(
            albumId
        );

        var album = await mediator.Send(query, ct);

        return Results.Ok(new AlbumHeaderDto(
            album.Id,
            album.Name,
            album.Description
        ));
    }

    private static async Task<IResult> GetAlbumImagesAsync(Guid albumId, int pageSize, int page, IMediator mediator,
        CancellationToken ct)
    {
        var skip = pageSize * page;
        var take = pageSize;

        var query = new GetAlbumPhotosQuery(
            albumId,
            skip,
            take
        );

        var response = await mediator.Send(query, ct);

        return response.IsSuccess ?
            TypedResults.Ok(response.Value) :
            TypedResults.NotFound();
    }

    private static async Task<IResult> GetAlbumsMetadataAsync(int pageSize, int page, IMediator mediator, CancellationToken ct)
    {
        var query = new GetAlbumsMetaDataQuery(
            pageSize,
            page
        );

        var result = await mediator.Send(query, ct);

        return result.IsSuccess ? 
            TypedResults.Ok(result.Value) : 
            TypedResults.BadRequest();
    }

    private static async Task<IResult> CreateAlbumAsync(CreateAlbumRequest request, IMediator mediator,
        CancellationToken ct)
    {
        var command = new CreateAlbumCommand(
            request.Name,
            request.Description
        );

        await mediator.Send(command, ct);

        return Results.NoContent();
    }

    private static async Task<IResult> DeleteAlbumAsync(Guid albumId, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteAlbumCommand(albumId), ct);

        return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.Errors);
    }

    private static async Task<IResult> AddImagesToAlbumAsync(Guid albumId, [FromBody] AddImagesToAlbumRequest request,
        IMediator mediator, CancellationToken ct)
    {
        var command = new AddImagesToAlbumCommand(
            albumId,
            request.ImageIds
        );
        
        await mediator.Send(command, ct);
        
        return Results.NoContent();
    }

    private static async Task<IResult> RemoveImagesFromAlbumAsync(Guid albumId,
        [FromBody] RemoveImagesFromAlbumRequest request, IMediator mediator, CancellationToken ct)
    {
        var command = new RemoveImagesFromAlbumCommand(
            albumId,
            request.ImageIds
        );

        var result = await mediator.Send(command, ct);

        return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.Errors);
    }
}