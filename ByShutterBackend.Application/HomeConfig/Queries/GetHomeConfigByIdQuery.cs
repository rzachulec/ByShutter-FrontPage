using ByShutterBackend.Application.HomeConfig.DTO;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Queries;

public record GetHomeConfigByIdQuery(
    Guid HomeConfigId
) : IRequest<HomeProfileDto>;