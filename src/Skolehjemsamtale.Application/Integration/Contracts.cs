using Skolehjemsamtale.Application.Dtos;

namespace Skolehjemsamtale.Application.Integration;

/// <summary>
/// Versionsstyrede integrationskontrakter udadtil (fx til Techtree).
/// Kontrakterne er bevidst løst koblet til domænet og må ikke ændres bagudkompatibelt uden versionsbump.
/// Alle id'er er Guids som tekst, og tidsstempler er ISO-8601 UTC, så modtageren kan være bygget i en anden teknologi.
/// </summary>
public static class IntegrationEventTypes
{
    public const string ObservationRegistreret = "observation.registreret";
    public const string SamtalePlanlagt = "samtale.planlagt";
    public const string SamtaleAfsluttet = "samtale.afsluttet";
    public const string AftaleOprettet = "aftale.oprettet";
    public const string HjemmeIndsigtRegistreret = "hjemmeindsigt.registreret";
}

public sealed record ObservationRegistreretV1(
    string ElevId, string ObservationId, string Dato, string Kategori, string Valens,
    string Beskrivelse, string Kontekst, string ObserveretAf);

public sealed record SamtalePlanlagtV1(
    string SamtaleId, string ElevId, string Type, string Dato, string Formaal, string SkabelonKode);

public sealed record SamtaleAfsluttetV1(
    string SamtaleId, string ElevId, string Dato, int AntalAftaler, int Kvalitetsscore,
    bool HjemInddraget, bool ElevInddraget);

public sealed record AftaleOprettetV1(
    string AftaleId, string SamtaleId, string ElevId, string Beskrivelse, string Ansvarlig,
    string? Frist, string? Evalueringsdato);

public sealed record HjemmeIndsigtRegistreretV1(
    string HjemmeIndsigtId, string ElevId, string? SamtaleId, string Kategori,
    string Spoergsmaal, string Svar, string BidragetAf);
