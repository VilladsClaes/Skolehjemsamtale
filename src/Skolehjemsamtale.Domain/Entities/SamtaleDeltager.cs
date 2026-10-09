using Skolehjemsamtale.Domain.Common;
using Skolehjemsamtale.Domain.Enums;

namespace Skolehjemsamtale.Domain.Entities;

/// <summary>Deltager i en samtale. Elevens deltagelse skal være et bevidst valg (inddragelse).</summary>
public class SamtaleDeltager : Entity
{
    public Guid SamtaleId { get; set; }
    public Samtale? Samtale { get; set; }

    public string Navn { get; set; } = string.Empty;
    public SamtaleRolle Rolle { get; set; }
    public bool Deltog { get; set; } = true;

    private SamtaleDeltager() { }

    public static SamtaleDeltager Opret(string navn, SamtaleRolle rolle)
        => new() { Navn = navn?.Trim() ?? string.Empty, Rolle = rolle };
}
