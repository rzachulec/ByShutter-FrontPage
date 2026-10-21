using ByShutterBackend.Application.Portfolio.DTO;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Queries.Photos;

public record GetPhotoQuery(
    Guid PhotoId
) : IRequest<PhotoDto>;