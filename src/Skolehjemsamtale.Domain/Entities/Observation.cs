using Skolehjemsamtale.Domain.Common;
using Skolehjemsamtale.Domain.Enums;

namespace Skolehjemsamtale.Domain.Entities;

/// <summary>
/// En observation af elevens adfærd nedfældet i undervisningen – grundlaget for samtalen.
/// Beskrivelsen skal være observerbar adfærd (hvad, hvor, hvornår) og må ikke være en diagnose eller tillagt motiv.
/// </summary>
public class Observation : Entity
{
    public Guid ElevId { get; set; }
    public Elev? Elev { get; set; }

    public DateOnly Dato { get; set; }
    public ObservationKategori Kategori { get; set; }
    public ObservationValens Valens { get; set; } = ObservationValens.Opmærksomhed;

    /// <summary>Observerbar adfærd i konkrete situationer – ikke tolkning eller mærkat.</summary>
    public string Beskrivelse { get; set; } = string.Empty;

    /// <summary>Kontekst: fag, lektion, situation (fx "Matematik, gruppearbejde").</summary>
    public string Kontekst { get; set; } = string.Empty;

    /// <summary>Hvad har allerede hjulpet? (styrkebaseret, jf. appreciative inquiry)</summary>
    public string? HvadHjalp { get; set; }

    public string ObserveretAf { get; set; } = string.Empty;

    /// <summary>Om observationen er delt med hjemmet. Understøtter "informeret vs. inddraget".</summary>
    public bool DeltMedHjem { get; set; }

    private Observation() { }

    public static Observation Registrer(
        Guid elevId, DateOnly dato, ObservationKategori kategori, ObservationValens valens,
        string beskrivelse, string kontekst, string observeretAf, string? hvadHjalp = null)
    {
        if (elevId == Guid.Empty) throw new DomainException("Observationen skal knyttes til en elev.");
        if (string.IsNullOrWhiteSpace(beskrivelse))
            throw new DomainException("Observationen skal have en beskrivelse.");
        if (string.IsNullOrWhiteSpace(observeretAf))
            throw new DomainException("Observatør skal angives.");

        return new Observation
        {
            ElevId = elevId,
            Dato = dato,
            Kategori = kategori,
            Valens = valens,
            Beskrivelse = beskrivelse.Trim(),
            Kontekst = kontekst?.Trim() ?? string.Empty,
            ObserveretAf = observeretAf.Trim(),
            HvadHjalp = string.IsNullOrWhiteSpace(hvadHjalp) ? null : hvadHjalp.Trim()
        };
    }

    public void MarkerDeltMedHjem()
    {
        DeltMedHjem = true;
        Opdateret = DateTimeOffset.UtcNow;
    }
}
