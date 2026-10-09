using Skolehjemsamtale.Application.Services;
using Skolehjemsamtale.Domain.Entities;
using Skolehjemsamtale.Domain.Enums;
using Xunit;

namespace Skolehjemsamtale.Tests;

public class SamtaleKvalitetTests
{
    [Fact]
    public void Fuld_samtale_opfylder_alle_principper()
    {
        var elevId = Guid.NewGuid();
        var styrke = Observation.Registrer(elevId, DateOnly.FromDateTime(DateTime.UtcNow),
            ObservationKategori.Faglig, ObservationValens.Styrke, "Gennemarbejdet oplæg", "Samfundsfag", "Lærer");
        var opmaerksomhed = Observation.Registrer(elevId, DateOnly.FromDateTime(DateTime.UtcNow),
            ObservationKategori.Social, ObservationValens.Opmærksomhed, "Afbrød gruppearbejde", "Dansk", "Lærer");

        var s = Samtale.Planlæg(elevId, SamtaleType.SkoleHjemSamtale, DateOnly.FromDateTime(DateTime.UtcNow), "Fælles forståelse");
        s.KoblObservation(styrke.Id);
        s.KoblObservation(opmaerksomhed.Id);
        s.TilføjNote(SamtaleFaseKode.InviterHjemmetsPerspektiv, Perspektiv.Hjem, "Ser uro i aftenerne.");
        s.TilføjNote(SamtaleFaseKode.ElevensPerspektiv, Perspektiv.Elev, "Gruppearbejde er svært.");
        s.TilføjNote(SamtaleFaseKode.FaellesForstaaelse, Perspektiv.Skole, "Fælles billede etableret.");
        s.TilføjNote(SamtaleFaseKode.StemmeTjek, Perspektiv.Skole, "Alle oplevede at blive hørt.");
        s.TilføjAftale("Skolen tager ansvar for fast plads", AftaleAnsvar.Skole, evalueringsdato: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)));
        s.TilføjAftale("Hjemmet noterer mønster", AftaleAnsvar.Hjem, evalueringsdato: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)));

        var k = SamtaleKvalitetEvaluator.Evaluer(s, new[] { styrke, opmaerksomhed });

        Assert.Equal(100, k.Score);
        Assert.All(k.Tjek, t => Assert.True(t.Opfyldt, $"Tjek '{t.Kode}' burde være opfyldt"));
    }

    [Fact]
    public void Tom_samtale_scores_lavt_og_peger_på_mangler()
    {
        var s = Samtale.Planlæg(Guid.NewGuid(), SamtaleType.Foraeldresamtale, DateOnly.FromDateTime(DateTime.UtcNow), "Formål");

        var k = SamtaleKvalitetEvaluator.Evaluer(s, Array.Empty<Observation>());

        Assert.True(k.Score < 50);
        Assert.Contains(k.Tjek, t => t.Kode == "hjem" && !t.Opfyldt);
        Assert.Contains(k.Tjek, t => t.Kode == "elev" && !t.Opfyldt);
    }
}
