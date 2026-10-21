using ByShutterBackend.Domain.HomeConfig;

namespace ByShutterBackend.Domain.Common.Interfaces;

public interface IHomeProfileRepository
{
    public Task<HomeProfile?> GetHomeProfileAsync(Guid id, CancellationToken ct = default);
    public Task<HomeProfile> CreateHomeProfileAsync(HomeProfile homeProfile, CancellationToken ct = default);
    public Task<HomeProfile> UpdateHomeProfileAsync(HomeProfile homeProfile, CancellationToken ct = default);
    public Task DeleteHomeProfileAsync(HomeProfile homeProfile, CancellationToken ct = default);
    public Task<IEnumerable<HomeProfile>> GetHomeProfilesAsync(int skip = 0, int take = 10,
        CancellationToken ct = default);
}