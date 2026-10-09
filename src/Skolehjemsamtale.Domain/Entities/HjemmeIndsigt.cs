using Skolehjemsamtale.Domain.Common;
using Skolehjemsamtale.Domain.Enums;

namespace Skolehjemsamtale.Domain.Entities;

/// <summary>
/// Ny data til læreren fra hjemmet – det skolen ikke kan se i skolekonteksten
/// (fx søvn, morgener, helbred, fritid, hvad eleven siger derhjemme).
/// Oplysningen er hjemmets perspektiv, ikke et bevis på årsag.
/// </summary>
public class HjemmeIndsigt : Entity
{
    public Guid ElevId { get; set; }
    public Elev? Elev { get; set; }

    /// <summary>Kobling til den samtale oplysningen blev indsamlet i (kan være null).</summary>
    public Guid? SamtaleId { get; set; }
    public Samtale? Samtale { get; set; }

    /// <summary>Spørgsmålet der blev stillet – gerne fra den researchbaserede spørgeramme.</summary>
    public string Spoergsmaal { get; set; } = string.Empty;

    /// <summary>Hjemmets svar, gengivet så tæt på deres egne ord som muligt.</summary>
    public string Svar { get; set; } = string.Empty;

    public ObservationKategori Kategori { get; set; } = ObservationKategori.Trivsel;

    /// <summary>Hvem fra hjemmet har bidraget (fx "Mor", "Far", "Elev + forælder").</summary>
    public string BidragetAf { get; set; } = string.Empty;

    /// <summary>Kilde til perspektivet. Altid Hjem.</summary>
    public Perspektiv Perspektiv { get; set; } = Perspektiv.Hjem;

    private HjemmeIndsigt() { }

    public static HjemmeIndsigt Registrer(
        Guid elevId, string spoergsmaal, string svar, ObservationKategori kategori, string bidragetAf, Guid? samtaleId = null)
    {
        if (string.IsNullOrWhiteSpace(svar))
            throw new DomainException("Hjemmeindsigten skal have et svar.");
        return new HjemmeIndsigt
        {
            ElevId = elevId,
            SamtaleId = samtaleId,
            Spoergsmaal = spoergsmaal?.Trim() ?? string.Empty,
            Svar = svar.Trim(),
            Kategori = kategori,
            BidragetAf = bidragetAf?.Trim() ?? string.Empty
        };
    }
}
