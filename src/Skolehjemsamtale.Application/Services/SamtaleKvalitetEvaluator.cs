using Skolehjemsamtale.Application.Dtos;
using Skolehjemsamtale.Domain.Entities;
using Skolehjemsamtale.Domain.Enums;

namespace Skolehjemsamtale.Application.Services;

/// <summary>
/// Evaluerer en samtale mod de bærende principper fra forskningen: inddragelse af hjem og elev,
/// styrkebaseret blik, fælles forståelse, delt ansvar, testbare aftaler og stemmetjek.
/// Vurderingen er vejledende og skal hjælpe læreren – ikke bedømme familien.
/// </summary>
public static class SamtaleKvalitetEvaluator
{
    public static SamtaleKvalitetDto Evaluer(Samtale s, IReadOnlyCollection<Observation> relateredeObservationer)
    {
        var noter = s.Noter;
        var aftaler = s.Aftaler;

        var tjek = new List<KvalitetsTjekDto>
        {
            new("observation", "Konkrete observationer er knyttet til samtalen", s.Observationer.Count > 0,
                s.Observationer.Count > 0 ? $"{s.Observationer.Count} observation(er) koblet." : "Ingen observationer er koblet endnu."),

            new("styrke", "Mindst én styrke/ressource er løftet", relateredeObservationer.Any(o => o.Valens == ObservationValens.Styrke),
                relateredeObservationer.Any(o => o.Valens == ObservationValens.Styrke) ? "Styrke registreret." : "Kun opmærksomheder – husk at løfte det, der virker."),

            new("hjem", "Hjemmets perspektiv er inddraget", s.HjemInddraget || noter.Any(n => n.Perspektiv == Perspektiv.Hjem),
                s.HjemInddraget ? "Hjemmets perspektiv indgår." : "Hjemmets oplevelse mangler."),

            new("elev", "Elevens stemme er inddraget", s.ElevInddraget || noter.Any(n => n.Perspektiv == Perspektiv.Elev),
                s.ElevInddraget ? "Elevens stemme indgår." : "Elevens synspunkt mangler."),

            new("faelles", "Der er skabt et fælles billede",
                noter.Any(n => n.Fase == SamtaleFaseKode.FaellesForstaaelse),
                noter.Any(n => n.Fase == SamtaleFaseKode.FaellesForstaaelse) ? "Fælles forståelse noteret." : "Fælles forståelse mangler."),

            new("aftale", "Der er aftalt et lille, testbart skridt", aftaler.Count > 0,
                aftaler.Count > 0 ? $"{aftaler.Count} aftale(r)." : "Ingen aftaler endnu."),

            new("deltansvar", "Ansvaret er ikke ensidigt",
                aftaler.Count > 0 && aftaler.Select(a => a.Ansvarlig).Distinct().Count() > 1,
                aftaler.Select(a => a.Ansvarlig).Distinct().Count() > 1 ? "Flere parter har ansvar." : "Overvej om ansvaret bliver ensidigt."),

            new("opfølgning", "Opfølgning er aftalt med dato",
                aftaler.Any(a => a.Evalueringsdato is not null),
                aftaler.Any(a => a.Evalueringsdato is not null) ? "Evalueringsdato sat." : "Aftal hvornår I følger op."),

            new("stemmetjek", "Deltagerne har fået tjekket, om de blev hørt",
                noter.Any(n => n.Fase == SamtaleFaseKode.StemmeTjek),
                noter.Any(n => n.Fase == SamtaleFaseKode.StemmeTjek) ? "Stemmetjek gennemført." : "Stemmetjek mangler.")
        };

        var opfyldt = tjek.Count(t => t.Opfyldt);
        var score = tjek.Count == 0 ? 0 : (int)Math.Round(100.0 * opfyldt / tjek.Count);
        return new SamtaleKvalitetDto(tjek, score, opfyldt, tjek.Count);
    }
}
