using Skolehjemsamtale.Domain.Common;
using Skolehjemsamtale.Domain.Enums;

namespace Skolehjemsamtale.Domain.Entities;

/// <summary>
/// En note knyttet til en fase i samtalen og til et perspektiv (skole, hjem eller elev).
/// At holde perspektiverne adskilt er et bærende princip: forskellige billeder er data, ikke fejl.
/// </summary>
public class SamtaleFaseNote : Entity
{
    public Guid SamtaleId { get; set; }
    public Samtale? Samtale { get; set; }

    public SamtaleFaseKode Fase { get; set; }
    public Perspektiv Perspektiv { get; set; } = Perspektiv.Skole;
    public string Indhold { get; set; } = string.Empty;

    /// <summary>Den konkrete observation noten bygger på (valgfrit).</summary>
    public Guid? ObservationId { get; set; }

    private SamtaleFaseNote() { }

    public static SamtaleFaseNote Opret(SamtaleFaseKode fase, Perspektiv perspektiv, string indhold, Guid? observationId = null)
    {
        if (string.IsNullOrWhiteSpace(indhold))
            throw new DomainException("Noten skal have et indhold.");
        return new SamtaleFaseNote
        {
            Fase = fase,
            Perspektiv = perspektiv,
            Indhold = indhold.Trim(),
            ObservationId = observationId
        };
    }
}
