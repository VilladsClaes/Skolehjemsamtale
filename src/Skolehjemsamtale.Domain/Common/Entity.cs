namespace Skolehjemsamtale.Domain.Common;

/// <summary>
/// Fælles base for alle domæneentiteter. Id er en Guid, så poster kan deles
/// på tværs af systemer (fx Techtree) uden at være afhængige af en lokal identity-kolonne.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTimeOffset Oprettet { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? Opdateret { get; set; }

    /// <summary>
    /// Stabilt, eksternt id når posten spejles/udveksles med et andet system (Techtree).
    /// Null indtil en ekstern kobling er etableret.
    /// </summary>
    public string? EksterntId { get; set; }
}
