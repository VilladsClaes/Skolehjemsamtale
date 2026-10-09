using Skolehjemsamtale.Domain.Common;

namespace Skolehjemsamtale.Infrastructure.Integration;

/// <summary>
/// Transaktionel outbox-post. Skrives i samme transaktion som domæneændringen,
/// så et integrationsevent til Techtree ikke kan gå tabt eller sendes uden at ændringen er gemt.
/// </summary>
public class OutboxMessage : Entity
{
    public string EventType { get; set; } = string.Empty;
    public int Versionsnummer { get; set; }
    public string AggregateType { get; set; } = string.Empty;
    public Guid AggregateId { get; set; }
    public Guid? ElevId { get; set; }
    public string Payload { get; set; } = "{}";
    /// <summary>Bestillingsnøgle for FIFO-levering. SQLite kan ikke sortere på DateTimeOffset, derfor en DateTime i UTC.</summary>
    public DateTime OprettetUtc { get; set; } = DateTime.UtcNow;

    public bool Behandlet { get; set; }
    public int Forsøg { get; set; }
    public DateTimeOffset? BehandletTid { get; set; }
    public string? Fejl { get; set; }
}
