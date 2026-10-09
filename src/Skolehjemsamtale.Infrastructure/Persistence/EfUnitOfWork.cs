using Microsoft.EntityFrameworkCore;
using Skolehjemsamtale.Application.Abstractions;

namespace Skolehjemsamtale.Infrastructure.Persistence;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly SkoleDbContext _db;
    public EfUnitOfWork(SkoleDbContext db) => _db = db;

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

    /// <summary>
    /// Kører flere ændringer i én transaktion, så domæneændring og outbox-post
    /// kan gemmes atomisk. Bruges når en operation omfatter flere gem-kald.
    /// </summary>
    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        await action(ct);
        await tx.CommitAsync(ct);
    }
}
