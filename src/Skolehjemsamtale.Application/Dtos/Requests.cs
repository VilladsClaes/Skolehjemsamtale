using Skolehjemsamtale.Domain.Enums;

namespace Skolehjemsamtale.Application.Dtos;

public sealed record OpretElevRequest(string Navn, string Klasse, DateOnly? Foedselsdato = null);

public sealed record OpdaterElevRequest(string Navn, string Klasse, bool Aktiv = true);

public sealed record RegistrerObservationRequest(
    Guid ElevId, DateOnly Dato, ObservationKategori Kategori, ObservationValens Valens,
    string Beskrivelse, string Kontekst, string ObserveretAf, string? HvadHjalp = null);

public sealed record DeltagerRequest(string Navn, SamtaleRolle Rolle, bool Deltog = true);

public sealed record PlanlaegSamtaleRequest(
    Guid ElevId, SamtaleType Type, DateOnly Dato, string Formaal,
    string SkabelonKode = "skole-hjem-grund", string? Sted = null,
    IReadOnlyList<Guid>? ObservationIds = null,
    IReadOnlyList<DeltagerRequest>? Deltagere = null);

public sealed record TilfoejNoteRequest(
    SamtaleFaseKode Fase, Perspektiv Perspektiv, string Indhold, Guid? ObservationId = null);

public sealed record TilfoejAftaleRequest(
    string Beskrivelse, AftaleAnsvar Ansvarlig, string? AnsvarligNavn = null,
    DateOnly? Frist = null, DateOnly? Evalueringsdato = null);

public sealed record TilfoejHjemmeIndsigtRequest(
    string Spoergsmaal, string Svar, ObservationKategori Kategori, string BidragetAf);

public sealed record AflysSamtaleRequest(string? Aarsag);

public sealed record EvaluerAftaleRequest(string Evaluering, bool Fortsaet);
