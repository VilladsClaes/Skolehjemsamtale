using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skolehjemsamtale.Infrastructure.Persistence;

namespace Skolehjemsamtale.Web.Controllers;

/// <summary>
/// Drift af koblingen til Techtree: status på outbox og manuel genafspilning.
/// Beskyttelse af dette endpoint sker i Techtree-/gateway-laget (se docs/integration).
/// </summary>
[ApiController]
[Route("api/v1/integration")]
public class IntegrationController : ControllerBase
{
    private readonly SkoleDbContext _db;
    public IntegrationController(SkoleDbContext db) => _db = db;

    [HttpGet("status")]
    public async Task<IActionResult> Status(CancellationToken ct)
    {
        var ventende = await _db.Outbox.CountAsync(m => !m.Behandlet, ct);
        var behandlede = await _db.Outbox.CountAsync(m => m.Behandlet, ct);
        var fejlede = await _db.Outbox.CountAsync(m => m.Fejl != null, ct);
        return Ok(new { ventende, behandlede, fejlede });
    }

    [HttpGet("outbox")]
    public async Task<IActionResult> Outbox([FromQuery] int take = 50, CancellationToken ct = default)
    {
        var items = await _db.Outbox
            .OrderByDescending(m => m.OprettetUtc)
            .Take(Math.Clamp(take, 1, 200))
            .Select(m => new { m.Id, m.EventType, m.Versionsnummer, m.AggregateType, m.AggregateId, m.ElevId, m.Behandlet, m.Forsøg, m.Fejl, m.Oprettet })
            .ToListAsync(ct);
        return Ok(items);
    }
}
