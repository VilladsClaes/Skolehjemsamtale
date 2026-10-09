using Skolehjemsamtale.Domain.Common;
using Skolehjemsamtale.Domain.Enums;

namespace Skolehjemsamtale.Domain.Entities;

/// <summary>
/// Samtalen er aggregatet. Den følger op på observationer fra undervisningen og
/// gennemfører en ærlig, vækstorienteret samtale, der fremmer forældrenes oplevelse af
/// samarbejde og giver eleven en reel stemme.
/// </summary>
public class Samtale : Entity
{
    public Guid ElevId { get; set; }
    public Elev? Elev { get; set; }

    public SamtaleType Type { get; set; }
    public SamtaleStatus Status { get; private set; } = SamtaleStatus.Planlagt;

    /// <summary>Kode for den anvendte samtalestruktur (se DefaultSamtaleSkabeloner).</summary>
    public string SkabelonKode { get; set; } = "skole-hjem-grund";

    /// <summary>Det fælles formål med samtalen, formuleret i klart sprog.</summary>
    public string Formaal { get; set; } = string.Empty;

    public DateOnly Dato { get; set; }
    public string? Sted { get; set; }

    /// <summary>Om eleven deltager/har fået sin stemme inddraget.</summary>
    public bool ElevInddraget { get; set; }

    /// <summary>Om hjemmets perspektiv er indhentet.</summary>
    public bool HjemInddraget { get; set; }

    public DateTimeOffset? Afholdt { get; private set; }
    public string? Aflysningsaarsag { get; private set; }

    private readonly List<SamtaleFaseNote> _noter = new();
    public IReadOnlyCollection<SamtaleFaseNote> Noter => _noter;

    private readonly List<SamtaleDeltager> _deltagere = new();
    public IReadOnlyCollection<SamtaleDeltager> Deltagere => _deltagere;

    private readonly List<Handlingsaftale> _aftaler = new();
    public IReadOnlyCollection<Handlingsaftale> Aftaler => _aftaler;

    private readonly List<SamtaleObservation> _observationer = new();
    public IReadOnlyCollection<SamtaleObservation> Observationer => _observationer;

    private readonly List<HjemmeIndsigt> _hjemmeIndsigter = new();
    public IReadOnlyCollection<HjemmeIndsigt> HjemmeIndsigter => _hjemmeIndsigter;

    private Samtale() { }

    public static Samtale Planlæg(
        Guid elevId, SamtaleType type, DateOnly dato, string formaal,
        string skabelonKode = "skole-hjem-grund", string? sted = null)
    {
        if (elevId == Guid.Empty) throw new DomainException("Samtalen skal knyttes til en elev.");
        if (string.IsNullOrWhiteSpace(formaal)) throw new DomainException("Samtalen skal have et formål.");

        return new Samtale
        {
            ElevId = elevId,
            Type = type,
            Dato = dato,
            Formaal = formaal.Trim(),
            SkabelonKode = string.IsNullOrWhiteSpace(skabelonKode) ? "skole-hjem-grund" : skabelonKode.Trim(),
            Sted = sted
        };
    }

    public SamtaleDeltager TilføjDeltager(string navn, SamtaleRolle rolle, bool deltog = true)
    {
        var d = SamtaleDeltager.Opret(navn, rolle);
        d.Deltog = deltog;
        _deltagere.Add(d);
        if (rolle == SamtaleRolle.Elev) ElevInddraget = true;
        if (rolle == SamtaleRolle.Foraelder) HjemInddraget = true;
        Opdateret = DateTimeOffset.UtcNow;
        return d;
    }

    public void KoblObservation(Guid observationId)
    {
        if (_observationer.Any(o => o.ObservationId == observationId)) return;
        _observationer.Add(SamtaleObservation.Opret(observationId));
        Opdateret = DateTimeOffset.UtcNow;
    }

    public SamtaleFaseNote TilføjNote(SamtaleFaseKode fase, Perspektiv perspektiv, string indhold, Guid? observationId = null)
    {
        if (Status == SamtaleStatus.Aflyst)
            throw new DomainException("Kan ikke tilføje noter til en aflyst samtale.");
        var note = SamtaleFaseNote.Opret(fase, perspektiv, indhold, observationId);
        _noter.Add(note);
        if (perspektiv == Perspektiv.Hjem) HjemInddraget = true;
        if (perspektiv == Perspektiv.Elev) ElevInddraget = true;
        Opdateret = DateTimeOffset.UtcNow;
        return note;
    }

    public Handlingsaftale TilføjAftale(
        string beskrivelse, AftaleAnsvar ansvarlig, string? ansvarligNavn = null,
        DateOnly? frist = null, DateOnly? evalueringsdato = null)
    {
        var aftale = Handlingsaftale.Opret(beskrivelse, ansvarlig, ansvarligNavn, frist, evalueringsdato);
        _aftaler.Add(aftale);
        Opdateret = DateTimeOffset.UtcNow;
        return aftale;
    }

    public HjemmeIndsigt TilføjHjemmeIndsigt(string spoergsmaal, string svar, ObservationKategori kategori, string bidragetAf)
    {
        var indsigt = HjemmeIndsigt.Registrer(ElevId, spoergsmaal, svar, kategori, bidragetAf, Id);
        _hjemmeIndsigter.Add(indsigt);
        HjemInddraget = true;
        Opdateret = DateTimeOffset.UtcNow;
        return indsigt;
    }

    public void MarkerAfholdt()
    {
        if (Status == SamtaleStatus.Aflyst) throw new DomainException("En aflyst samtale kan ikke markeres afholdt.");
        Status = SamtaleStatus.Afholdt;
        Afholdt = DateTimeOffset.UtcNow;
        Opdateret = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Afslutter samtalen. Kræver, at der er lavet mindst én aftale og foretaget et stemmetjek,
    /// så værktøjet ikke lukker en samtale uden fælles næste skridt og inddragelse.
    /// </summary>
    public void Afslut()
    {
        if (Status == SamtaleStatus.Aflyst) throw new DomainException("En aflyst samtale kan ikke afsluttes.");
        if (_aftaler.Count == 0)
            throw new DomainException("Samtalen kan ikke afsluttes uden mindst én handlingsaftale.");
        if (!_noter.Any(n => n.Fase == SamtaleFaseKode.StemmeTjek))
            throw new DomainException("Samtalen kan ikke afsluttes uden et stemmetjek (oplevede deltagerne sig hørt?).");
        Status = SamtaleStatus.Afsluttet;
        Opdateret = DateTimeOffset.UtcNow;
    }

    public void Aflys(string aarsag)
    {
        Status = SamtaleStatus.Aflyst;
        Aflysningsaarsag = aarsag?.Trim();
        Opdateret = DateTimeOffset.UtcNow;
    }
}
