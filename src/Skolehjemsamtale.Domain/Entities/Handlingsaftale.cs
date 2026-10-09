using Skolehjemsamtale.Domain.Common;
using Skolehjemsamtale.Domain.Enums;

namespace Skolehjemsamtale.Domain.Entities;

/// <summary>
/// En lille, testbar aftale: hvem gør hvad, hvornår, og hvornår den evalueres.
/// Ansvaret må ikke blive ensidigt – derfor et eksplicit ansvarsfelt.
/// </summary>
public class Handlingsaftale : Entity
{
    public Guid SamtaleId { get; set; }
    public Samtale? Samtale { get; set; }

    public string Beskrivelse { get; set; } = string.Empty;
    public AftaleAnsvar Ansvarlig { get; set; }
    public string? AnsvarligNavn { get; set; }
    public DateOnly? Frist { get; set; }
    public AftaleStatus Status { get; set; } = AftaleStatus.Foreslaaet;

    /// <summary>Dato for aftalt opfølgning/evaluering.</summary>
    public DateOnly? Evalueringsdato { get; set; }

    public string? Evaluering { get; set; }

    private Handlingsaftale() { }

    public static Handlingsaftale Opret(
        string beskrivelse, AftaleAnsvar ansvarlig, string? ansvarligNavn = null,
        DateOnly? frist = null, DateOnly? evalueringsdato = null)
    {
        if (string.IsNullOrWhiteSpace(beskrivelse))
            throw new DomainException("Aftalen skal have en beskrivelse.");
        return new Handlingsaftale
        {
            Beskrivelse = beskrivelse.Trim(),
            Ansvarlig = ansvarlig,
            AnsvarligNavn = ansvarligNavn,
            Frist = frist,
            Evalueringsdato = evalueringsdato
        };
    }

    public void Aktivér()
    {
        Status = AftaleStatus.Aktiv;
        Opdateret = DateTimeOffset.UtcNow;
    }

    public void Evaluer(string evaluering, bool fortsæt)
    {
        Evaluering = evaluering?.Trim();
        Status = fortsæt ? AftaleStatus.Aktiv : AftaleStatus.Afsluttet;
        Opdateret = DateTimeOffset.UtcNow;
    }
}
