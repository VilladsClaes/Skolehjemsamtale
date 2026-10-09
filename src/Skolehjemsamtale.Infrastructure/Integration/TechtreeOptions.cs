namespace Skolehjemsamtale.Infrastructure.Integration;

/// <summary>
/// Konfiguration af koblingen til Techtree. Alt er valgfrit, så værktøjet kan køre
/// selvstændigt, indtil integrationen er på plads.
/// </summary>
public class TechtreeOptions
{
    public const string SectionName = "Techtree";

    /// <summary>Base-URL til Techtree. Tom = integration deaktiveret (events bliver liggende i outbox).</summary>
    public string? BaseUrl { get; set; }

    /// <summary>Relativ sti events postes til.</summary>
    public string EventsPath { get; set; } = "/api/integration/v1/events";

    /// <summary>Navn på den delte hemmelighed (miljøvariabel/secret), ikke selve værdien.</summary>
    public string? SharedSecretName { get; set; }

    /// <summary>Om outbox-processoren er aktiv.</summary>
    public bool Enabled { get; set; } = true;

    public int BatchSize { get; set; } = 25;
    public int MaksForsoeg { get; set; } = 8;
}
