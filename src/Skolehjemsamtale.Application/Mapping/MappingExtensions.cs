using Skolehjemsamtale.Application.Dtos;
using Skolehjemsamtale.Domain.Entities;
using Skolehjemsamtale.Domain.Framework;

namespace Skolehjemsamtale.Application.Mapping;

/// <summary>
/// Kortlægning fra domæne til DTO'er. DTO'erne er den stabile kontrakt udadtil
/// (API + Techtree), så domænet kan ændres uden at bryde integrationer.
/// </summary>
public static class MappingExtensions
{
    public static ObservationDto ToDto(this Observation o) => new(
        o.Id, o.ElevId, o.Dato, o.Kategori, o.Valens, o.Beskrivelse, o.Kontekst, o.HvadHjalp, o.ObserveretAf, o.DeltMedHjem);

    public static ElevDto ToDto(this Elev e, int antalObservationer = 0, int antalSamtaler = 0) => new(
        e.Id, e.Navn, e.Klasse, e.Foedselsdato, e.Aktiv, e.EksterntId, antalObservationer, antalSamtaler);

    public static SamtaleDeltagerDto ToDto(this SamtaleDeltager d) => new(d.Id, d.Navn, d.Rolle, d.Deltog);

    public static SamtaleFaseNoteDto ToDto(this SamtaleFaseNote n) => new(n.Id, n.Fase, n.Perspektiv, n.Indhold, n.ObservationId);

    public static HandlingsaftaleDto ToDto(this Handlingsaftale a) => new(
        a.Id, a.Beskrivelse, a.Ansvarlig, a.AnsvarligNavn, a.Frist, a.Status, a.Evalueringsdato, a.Evaluering);

    public static HjemmeIndsigtDto ToDto(this HjemmeIndsigt h) => new(
        h.Id, h.ElevId, h.SamtaleId, h.Spoergsmaal, h.Svar, h.Kategori, h.BidragetAf, h.Oprettet);

    public static SamtaleDto ToDto(this Samtale s, string? elevNavn = null) => new(
        s.Id, s.ElevId, elevNavn ?? s.Elev?.Navn ?? string.Empty, s.Type, s.Status, s.SkabelonKode,
        s.Formaal, s.Dato, s.ElevInddraget, s.HjemInddraget, s.Afholdt, s.Aftaler.Count);

    public static SamtaleTrinDto ToDto(this SamtaleTrin t) => new(
        t.Kode, t.Titel, t.Formaal, t.PrimaertPerspektiv, t.Spoergsmaal, t.PasPaa);

    public static SamtaleSkabelonDto ToDto(this SamtaleSkabelon s) => new(
        s.Kode, s.Navn, s.Beskrivelse, s.AnbefaletType, s.Trin.Select(t => t.ToDto()).ToList());

    public static HjemmeSpoergsmaalDto ToDto(this HjemmeSpoergsmaal h) => new(h.Spoergsmaal, h.Kategori);
}
