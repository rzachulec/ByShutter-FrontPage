using ByShutterBackend.Application.Portfolio.DTO;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Queries.Photos;

public record GetPhotosBatchQuery(
    int PageSize,
    int Page
) : IRequest<IEnumerable<PhotoDto>>;