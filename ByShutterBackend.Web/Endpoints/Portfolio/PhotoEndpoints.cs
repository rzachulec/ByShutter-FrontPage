using ByShutterBackend.Application.FileStorage.Commands;
using ByShutterBackend.Application.HomeConfig.DTO;
using ByShutterBackend.Application.Portfolio.Commands;
using ByShutterBackend.Application.Portfolio.Commands.Photos;
using ByShutterBackend.Application.Portfolio.Queries.Photos;
using ByShutterBackend.Web.Common;
using ByShutterBackend.Web.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ByShutterBackend.Web.Endpoints.Portfolio;

public static class PhotoEndpoints
{
    public static IEndpointRouteBuilder MapPhotoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/photos");

        group.MapPost("/", UploadPhotoAsync).RequireAuthorization().DisableAntiforgery();

        group.MapGet("/{photoId:guid}", GetPhotoAsync);
        group.MapGet("/{fileName}", GetPhotoByFileNameAsync);
        group.MapGet("/{pageSize:int}/{page:int}", GetPhotosAsync);
        group.MapGet("/{albumId:guid}/avaiable-photos", GetPhotosNotIn)
            .Produces<IEnumerable<PhotoDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        
        group.MapPut("/{photoId:guid}", UpdatePhotoAsync).RequireAuthorization();

        group.MapDelete("/{photoId:guid}", DeletePhotoAsync).RequireAuthorization();
        return app;
    }

    public static async Task<IResult> UploadPhotoAsync([FromForm] UploadPhotoRequest request, IMediator mediator,
        IOptions<ImageStoragePathOptions> options, CancellationToken ct)
    {
        var stream = request.Photo.OpenReadStream();
        var meta = new CreatePhotoMetaCommand(
            request.FileName,
            options.Value.Directory,
            request.ImageAlt,
            request.DisplayName ?? string.Empty,
            request.Description ?? string.Empty
        );
        var image = new UploadImageCommand(
            stream,
            request.FileName,
            options.Value.Directory
        );

        await mediator.Send(meta, ct);
        await mediator.Send(image, ct);

        return Results.NoContent();
    }

    private static async Task<IResult> GetPhotoAsync(Guid photoId, IMediator mediator, CancellationToken ct)
    {
        var query = new GetPhotoQuery(photoId);

        var photo = await mediator.Send(query, ct);

        return Results.Ok(photo);
    }

    private static async Task<IResult> GetPhotoByFileNameAsync(string fileName, IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetPhotoByFileNameQuery(fileName);

        var photo = await mediator.Send(query, ct);

        return Results.Ok(photo);
    }

    private static async Task<IResult> GetPhotosAsync(int pageSize, int page, IMediator mediator, CancellationToken ct)
    {
        var query = new GetPhotosBatchQuery(
            pageSize,
            page
        );

        var batch = await mediator.Send(query, ct);

        return Results.Ok(batch);
    }

    private static async Task<IResult> GetPhotosNotIn(Guid albumId, int page, int pageSize, IMediator mediator, CancellationToken ct)
    {
        var query = new GetPhotosNotInAlbumPagedQuery(
            albumId,
            page * pageSize,
            pageSize
        );

        var response = await mediator.Send(query, ct);

        return response.IsSuccess ?
            TypedResults.Ok(response.Value) :
            TypedResults.NotFound();
    }

    private static async Task<IResult> UpdatePhotoAsync(Guid photoId, UpdatePhotoRequest request, IMediator mediator,
        CancellationToken ct)
    {
        var query = new UpdatePhotoCommand(
            photoId,
            request.DisplayName,
            request.Description,
            request.PhotoAlt
        );
        
        await mediator.Send(query, ct);
        
        return Results.NoContent();
    }

    private static async Task<IResult> DeletePhotoAsync(Guid photoId, IMediator mediator, CancellationToken ct)
    {
        var query = new DeleteImageCommand(photoId);
        
        await mediator.Send(query, ct);
        
        return Results.NoContent();
    }
}