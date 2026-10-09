using Skolehjemsamtale.Domain.Enums;

namespace Skolehjemsamtale.Application.Dtos;

public sealed record ElevDto(
    Guid Id, string Navn, string Klasse, DateOnly? Foedselsdato, bool Aktiv,
    string? EksterntId, int AntalObservationer, int AntalSamtaler);

public sealed record ObservationDto(
    Guid Id, Guid ElevId, DateOnly Dato, ObservationKategori Kategori, ObservationValens Valens,
    string Beskrivelse, string Kontekst, string? HvadHjalp, string ObserveretAf, bool DeltMedHjem);

public sealed record SamtaleDeltagerDto(Guid Id, string Navn, SamtaleRolle Rolle, bool Deltog);

public sealed record SamtaleFaseNoteDto(
    Guid Id, SamtaleFaseKode Fase, Perspektiv Perspektiv, string Indhold, Guid? ObservationId);

public sealed record HandlingsaftaleDto(
    Guid Id, string Beskrivelse, AftaleAnsvar Ansvarlig, string? AnsvarligNavn,
    DateOnly? Frist, AftaleStatus Status, DateOnly? Evalueringsdato, string? Evaluering);

public sealed record HjemmeIndsigtDto(
    Guid Id, Guid ElevId, Guid? SamtaleId, string Spoergsmaal, string Svar,
    ObservationKategori Kategori, string BidragetAf, DateTimeOffset Oprettet);

public sealed record SamtaleDto(
    Guid Id, Guid ElevId, string ElevNavn, SamtaleType Type, SamtaleStatus Status,
    string SkabelonKode, string Formaal, DateOnly Dato, bool ElevInddraget, bool HjemInddraget,
    DateTimeOffset? Afholdt, int AntalAftaler);

/// <summary>
/// Fuld samtale inkl. struktur (trin/vejledning), noter pr. perspektiv, aftaler og hjemmeindsigter.
/// </summary>
public sealed record SamtaleDetaljerDto(
    SamtaleDto Samtale,
    IReadOnlyList<SamtaleDeltagerDto> Deltagere,
    IReadOnlyList<SamtaleFaseNoteDto> Noter,
    IReadOnlyList<HandlingsaftaleDto> Aftaler,
    IReadOnlyList<HjemmeIndsigtDto> HjemmeIndsigter,
    IReadOnlyList<ObservationDto> RelateredeObservationer,
    SamtaleSkabelonDto Skabelon,
    SamtaleKvalitetDto Kvalitet);

public sealed record SamtaleTrinDto(
    SamtaleFaseKode Kode, string Titel, string Formaal, Perspektiv PrimaertPerspektiv,
    IReadOnlyList<string> Spoergsmaal, IReadOnlyList<string> PasPaa);

public sealed record SamtaleSkabelonDto(
    string Kode, string Navn, string Beskrivelse, SamtaleType AnbefaletType,
    IReadOnlyList<SamtaleTrinDto> Trin);

/// <summary>Kvalitetsvurdering af samtalens inddragelse, baseret på forskningens principper.</summary>
public sealed record KvalitetsTjekDto(string Kode, string Titel, bool Opfyldt, string Begrundelse);

public sealed record SamtaleKvalitetDto(IReadOnlyList<KvalitetsTjekDto> Tjek, int Score, int AntalOpfyldt, int AntalIAlt);

public sealed record HjemmeSpoergsmaalDto(string Spoergsmaal, ObservationKategori Kategori);

public sealed record ElevOverblikDto(
    ElevDto Elev,
    IReadOnlyList<SamtaleDto> Samtaler,
    IReadOnlyList<ObservationDto> SenesteObservationer,
    IReadOnlyList<HjemmeIndsigtDto> HjemmeIndsigter,
    int AabneAftaler);
