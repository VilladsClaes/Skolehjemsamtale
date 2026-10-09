using System.Text.Encodings.Web;
using System.Text.Json;
using Skolehjemsamtale.Application.Abstractions;
using Skolehjemsamtale.Infrastructure.Persistence;

namespace Skolehjemsamtale.Infrastructure.Integration;

/// <summary>
/// Skriver integrationsevents til outbox-tabellen. Levering til Techtree sker asynkront
/// af <see cref="OutboxProcessor"/>, så en utilgængelig modtager aldrig blokerer lærerens arbejde.
/// </summary>
public class EfIntegrationPublisher : IIntegrationPublisher
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        // Bevar danske tegn læsbart i payloaden (ikke \u00e6-form), så events er til at inspicere og logge.
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    private readonly SkoleDbContext _db;

    public EfIntegrationPublisher(SkoleDbContext db) => _db = db;

    public async Task PublicerAsync(IntegrationEvent evt, CancellationToken ct = default)
    {
        var msg = new OutboxMessage
        {
            EventType = evt.EventType,
            Versionsnummer = evt.Versionsnummer,
            AggregateType = evt.AggregateType,
            AggregateId = evt.AggregateId,
            ElevId = evt.ElevId,
            Payload = JsonSerializer.Serialize(evt.Payload, evt.Payload.GetType(), JsonOptions),
            OprettetUtc = DateTime.UtcNow
        };
        await _db.Outbox.AddAsync(msg, ct);
        await _db.SaveChangesAsync(ct);
    }
}
