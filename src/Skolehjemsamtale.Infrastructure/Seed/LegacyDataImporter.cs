using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Skolehjemsamtale.Domain.Entities;
using Skolehjemsamtale.Domain.Enums;
using Skolehjemsamtale.Infrastructure.Persistence;

namespace Skolehjemsamtale.Infrastructure.Seed;

/// <summary>
/// Importerer data fra det oprindelige samtaleværktøj. Bruges både til seed og til
/// en engangs-migrering, når det gamle JSON-format (samtale_vaerktoej_data.json) skal ind.
/// </summary>
public class LegacyDataImporter
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private readonly SkoleDbContext _db;
    public LegacyDataImporter(SkoleDbContext db) => _db = db;

    public static SeedRoot Parse(string json) =>
        JsonSerializer.Deserialize<SeedRoot>(json, Options) ?? new SeedRoot();

    public async Task<int> ImporterAsync(SeedRoot root, CancellationToken ct = default)
    {
        var antal = 0;
        foreach (var s in root.Elever)
        {
            var elev = Elev.Opret(
                s.Navn, s.Klasse,
                DateOnly.TryParse(s.Foedselsdato, out var fd) ? fd : null);

            var observationer = new List<Observation>();
            foreach (var o in s.Observationer)
            {
                var obs = Observation.Registrer(
                    elev.Id,
                    DateOnly.TryParse(o.Dato, out var d) ? d : DateOnly.FromDateTime(DateTime.UtcNow),
                    ParseEnum(o.Kategori, ObservationKategori.Trivsel),
                    ParseEnum(o.Valens, ObservationValens.Opmærksomhed),
                    string.IsNullOrWhiteSpace(o.Beskrivelse) ? "(ingen beskrivelse)" : o.Beskrivelse,
                    o.Kontekst, string.IsNullOrWhiteSpace(o.ObserveretAf) ? "Lærer" : o.ObserveretAf, o.HvadHjalp);
                if (o.DeltMedHjem) obs.MarkerDeltMedHjem();
                observationer.Add(obs);
            }
            _db.Observationer.AddRange(observationer);

            foreach (var st in s.Samtaler)
            {
                var dato = DateOnly.TryParse(st.Dato, out var sd) ? sd : DateOnly.FromDateTime(DateTime.UtcNow);
                var samtale = Samtale.Planlæg(
                    elev.Id, ParseEnum(st.Type, SamtaleType.SkoleHjemSamtale), dato,
                    string.IsNullOrWhiteSpace(st.Formaal) ? "Opfølgning på observationer fra undervisningen" : st.Formaal,
                    st.SkabelonKode, st.Sted);

                foreach (var obs in observationer) samtale.KoblObservation(obs.Id);
                foreach (var d in st.Deltagere) samtale.TilføjDeltager(d.Navn, ParseEnum(d.Rolle, SamtaleRolle.Foraelder));
                foreach (var n in st.Noter)
                    samtale.TilføjNote(ParseEnum(n.Fase, SamtaleFaseKode.Aabning), ParseEnum(n.Perspektiv, Perspektiv.Skole), n.Indhold);
                foreach (var a in st.Aftaler)
                    samtale.TilføjAftale(a.Beskrivelse, ParseEnum(a.Ansvarlig, AftaleAnsvar.Faelles), a.AnsvarligNavn,
                        DateOnly.TryParse(a.Frist, out var f) ? f : null,
                        DateOnly.TryParse(a.Evalueringsdato, out var ed) ? ed : null);
                foreach (var h in st.HjemmeIndsigter)
                    samtale.TilføjHjemmeIndsigt(h.Spoergsmaal, h.Svar, ParseEnum(h.Kategori, ObservationKategori.Trivsel), h.BidragetAf);

                ApplyStatus(samtale, st.Status);
                _db.Samtaler.Add(samtale);
                antal++;
            }

            _db.Elever.Add(elev);
        }
        await _db.SaveChangesAsync(ct);
        return antal;
    }

    private static void ApplyStatus(Samtale samtale, string status)
    {
        var s = ParseEnum(status, SamtaleStatus.Planlagt);
        if (s == SamtaleStatus.Afholdt) samtale.MarkerAfholdt();
        else if (s == SamtaleStatus.Afsluttet)
        {
            samtale.MarkerAfholdt();
            // Afslut kræver aftale + stemmetjek; spring over hvis data mangler.
            try { samtale.Afslut(); } catch (Domain.Common.DomainException) { }
        }
        else if (s == SamtaleStatus.Aflyst) samtale.Aflys("Importeret som aflyst.");
    }

    private static T ParseEnum<T>(string? value, T fallback) where T : struct, Enum =>
        Enum.TryParse<T>(value, ignoreCase: true, out var parsed) ? parsed : fallback;
}
