using ByShutterBackend.Application.HomeConfig.DTO;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Queries;

public record GetHomeProfilesQuery(int PageSize, int Page) : IRequest<IEnumerable<HomeProfileDto>>;