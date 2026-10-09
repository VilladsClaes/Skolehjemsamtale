using Microsoft.EntityFrameworkCore;
using Skolehjemsamtale.Application.Abstractions;
using Skolehjemsamtale.Domain.Entities;

namespace Skolehjemsamtale.Infrastructure.Persistence.Repositories;

public class ElevRepository : IElevRepository
{
    private readonly SkoleDbContext _db;
    public ElevRepository(SkoleDbContext db) => _db = db;

    public Task<Elev?> GetAsync(Guid id, CancellationToken ct = default) =>
        _db.Elever.FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task<Elev?> GetMedAltAsync(Guid id, CancellationToken ct = default) =>
        _db.Elever
            .Include(e => e.Observationer)
            .Include(e => e.Samtaler).ThenInclude(s => s.Aftaler)
            .Include(e => e.HjemmeIndsigter)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<Elev>> ListAsync(bool? aktiv = null, string? klasse = null, CancellationToken ct = default)
    {
        var q = _db.Elever
            .Include(e => e.Observationer)
            .Include(e => e.Samtaler)
            .AsQueryable();
        if (aktiv is not null) q = q.Where(e => e.Aktiv == aktiv);
        if (!string.IsNullOrWhiteSpace(klasse)) q = q.Where(e => e.Klasse == klasse);
        return await q.OrderBy(e => e.Klasse).ThenBy(e => e.Navn).ToListAsync(ct);
    }

    public async Task AddAsync(Elev elev, CancellationToken ct = default) => await _db.Elever.AddAsync(elev, ct);
    public void Remove(Elev elev) => _db.Elever.Remove(elev);
}
