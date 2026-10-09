using Microsoft.EntityFrameworkCore;
using Skolehjemsamtale.Application.Abstractions;
using Skolehjemsamtale.Domain.Entities;

namespace Skolehjemsamtale.Infrastructure.Persistence.Repositories;

public class ObservationRepository : IObservationRepository
{
    private readonly SkoleDbContext _db;
    public ObservationRepository(SkoleDbContext db) => _db = db;

    public Task<Observation?> GetAsync(Guid id, CancellationToken ct = default) =>
        _db.Observationer.FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IReadOnlyList<Observation>> ListForElevAsync(Guid elevId, CancellationToken ct = default) =>
        await _db.Observationer.Where(o => o.ElevId == elevId).ToListAsync(ct);

    public async Task<IReadOnlyList<Observation>> ListByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct = default)
    {
        if (ids.Count == 0) return Array.Empty<Observation>();
        return await _db.Observationer.Where(o => ids.Contains(o.Id)).ToListAsync(ct);
    }

    public async Task AddAsync(Observation observation, CancellationToken ct = default) =>
        await _db.Observationer.AddAsync(observation, ct);
}
