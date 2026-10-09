using Microsoft.EntityFrameworkCore;
using Skolehjemsamtale.Application.Abstractions;
using Skolehjemsamtale.Domain.Entities;

namespace Skolehjemsamtale.Infrastructure.Persistence.Repositories;

public class SamtaleRepository : ISamtaleRepository
{
    private readonly SkoleDbContext _db;
    public SamtaleRepository(SkoleDbContext db) => _db = db;

    public Task<Samtale?> GetAsync(Guid id, CancellationToken ct = default) =>
        _db.Samtaler.FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<Samtale?> GetMedDetaljerAsync(Guid id, CancellationToken ct = default) =>
        _db.Samtaler
            .Include(s => s.Elev)
            .Include(s => s.Noter)
            .Include(s => s.Deltagere)
            .Include(s => s.Aftaler)
            .Include(s => s.Observationer)
            .Include(s => s.HjemmeIndsigter)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<Samtale>> ListAsync(Guid? elevId = null, CancellationToken ct = default)
    {
        var q = _db.Samtaler.Include(s => s.Elev).Include(s => s.Aftaler).AsQueryable();
        if (elevId is not null) q = q.Where(s => s.ElevId == elevId);
        return await q.ToListAsync(ct);
    }

    public async Task AddAsync(Samtale samtale, CancellationToken ct = default) =>
        await _db.Samtaler.AddAsync(samtale, ct);
}
