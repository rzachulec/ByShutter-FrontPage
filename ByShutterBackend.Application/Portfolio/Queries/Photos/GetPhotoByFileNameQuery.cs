using ByShutterBackend.Application.Portfolio.DTO;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Queries.Photos;

public record GetPhotoByFileNameQuery(
    string FileName
) : IRequest<PhotoDto>;