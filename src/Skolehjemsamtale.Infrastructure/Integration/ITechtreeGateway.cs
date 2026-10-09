namespace Skolehjemsamtale.Infrastructure.Integration;

/// <summary>
/// Anti-corruption-lag mod Techtree: alt, der vedrører modtagerens protokol og format,
/// samles her, så resten af systemet ikke kender Techtrees teknologi.
/// </summary>
public interface ITechtreeGateway
{
    bool ErAktiveret { get; }
    Task LeverAsync(OutboxMessage message, CancellationToken ct = default);
}
