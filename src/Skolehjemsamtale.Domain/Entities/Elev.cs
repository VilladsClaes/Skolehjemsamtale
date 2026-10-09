using Skolehjemsamtale.Domain.Common;

namespace Skolehjemsamtale.Domain.Entities;

/// <summary>En elev. Minimal og systemuafhængig, så den kan spejles fra/til fx Techtree.</summary>
public class Elev : Entity
{
    public string Navn { get; set; } = string.Empty;
    public string Klasse { get; set; } = string.Empty;
    public DateOnly? Foedselsdato { get; set; }
    public bool Aktiv { get; set; } = true;

    private readonly List<Observation> _observationer = new();
    public IReadOnlyCollection<Observation> Observationer => _observationer;

    private readonly List<Samtale> _samtaler = new();
    public IReadOnlyCollection<Samtale> Samtaler => _samtaler;

    private readonly List<HjemmeIndsigt> _hjemmeIndsigter = new();
    public IReadOnlyCollection<HjemmeIndsigt> HjemmeIndsigter => _hjemmeIndsigter;

    private Elev() { }

    public static Elev Opret(string navn, string klasse, DateOnly? foedselsdato = null)
    {
        if (string.IsNullOrWhiteSpace(navn))
            throw new DomainException("Elevens navn skal angives.");
        return new Elev { Navn = navn.Trim(), Klasse = klasse?.Trim() ?? string.Empty, Foedselsdato = foedselsdato };
    }

    public void Opdater(string navn, string klasse, bool aktiv)
    {
        if (string.IsNullOrWhiteSpace(navn))
            throw new DomainException("Elevens navn skal angives.");
        Navn = navn.Trim();
        Klasse = klasse?.Trim() ?? string.Empty;
        Aktiv = aktiv;
        Opdateret = DateTimeOffset.UtcNow;
    }
}
