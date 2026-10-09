using Microsoft.EntityFrameworkCore;
using Skolehjemsamtale.Application.Abstractions;
using Skolehjemsamtale.Domain.Entities;

namespace Skolehjemsamtale.Infrastructure.Persistence.Repositories;

public class HjemmeIndsigtRepository : IHjemmeIndsigtRepository
{
    private readonly SkoleDbContext _db;
    public HjemmeIndsigtRepository(SkoleDbContext db) => _db = db;

    public async Task<IReadOnlyList<HjemmeIndsigt>> ListForElevAsync(Guid elevId, CancellationToken ct = default) =>
        await _db.HjemmeIndsigter.Where(h => h.ElevId == elevId).ToListAsync(ct);
}
