using Microsoft.Extensions.Logging;

namespace Skolehjemsamtale.Infrastructure.Integration;

/// <summary>
/// Gateway der bruges når Techtree ikke er konfigureret. Den fejler bevidst ikke,
/// men logger, så outbox-poster bliver liggende og kan leveres senere.
/// </summary>
public class NoopTechtreeGateway : ITechtreeGateway
{
    private readonly ILogger<NoopTechtreeGateway> _log;
    public NoopTechtreeGateway(ILogger<NoopTechtreeGateway> log) => _log = log;

    public bool ErAktiveret => false;

    public Task LeverAsync(OutboxMessage message, CancellationToken ct = default)
    {
        _log.LogDebug("Techtree er ikke konfigureret; event {EventType} ({Id}) forbliver i outbox.",
            message.EventType, message.Id);
        return Task.CompletedTask;
    }
}
