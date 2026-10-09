namespace Skolehjemsamtale.Application.Abstractions;

/// <summary>
/// En integrationbesked, der skal kunne leveres til et andet system (fx Techtree).
/// Kontrakten er bevidst generisk og versionsstyret, så den ikke antager, hvordan
/// modtageren er bygget.
/// </summary>
public sealed record IntegrationEvent
{
    public required string EventType { get; init; }
    public required int Versionsnummer { get; init; }
    public required string AggregateType { get; init; }
    public required Guid AggregateId { get; init; }
    public Guid? ElevId { get; init; }
    public required object Payload { get; init; }
    public DateTimeOffset Tidspunkt { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Publicerer integrationsevents via det transaktionelle outbox-mønster, så
/// koblingen til Techtree kan etableres (eller fejle) uden at blokere domæneoperationer.
/// </summary>
public interface IIntegrationPublisher
{
    Task PublicerAsync(IntegrationEvent evt, CancellationToken ct = default);
}
