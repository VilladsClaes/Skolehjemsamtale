using Skolehjemsamtale.Domain.Common;
using Skolehjemsamtale.Domain.Entities;
using Skolehjemsamtale.Domain.Enums;
using Xunit;

namespace Skolehjemsamtale.Tests;

public class SamtaleTests
{
    private static Samtale NySamtale() =>
        Samtale.Planlæg(Guid.NewGuid(), SamtaleType.SkoleHjemSamtale,
            DateOnly.FromDateTime(DateTime.UtcNow), "Forstå og aftale næste skridt");

    [Fact]
    public void Kan_ikke_afslutte_uden_handlingsaftale()
    {
        var s = NySamtale();
        s.TilføjNote(SamtaleFaseKode.StemmeTjek, Perspektiv.Skole, "Deltagerne følte sig hørt.");

        var ex = Assert.Throws<DomainException>(() => s.Afslut());
        Assert.Contains("handlingsaftale", ex.Message);
    }

    [Fact]
    public void Kan_ikke_afslutte_uden_stemmetjek()
    {
        var s = NySamtale();
        s.TilføjAftale("Fast kontaktperson om morgenen", AftaleAnsvar.Skole);

        var ex = Assert.Throws<DomainException>(() => s.Afslut());
        Assert.Contains("stemmetjek", ex.Message);
    }

    [Fact]
    public void Afslut_kraever_baade_aftale_og_stemmetjek()
    {
        var s = NySamtale();
        s.TilføjAftale("Lille, testbart skridt", AftaleAnsvar.Faelles, evalueringsdato: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)));
        s.TilføjNote(SamtaleFaseKode.StemmeTjek, Perspektiv.Skole, "Begge oplevede at blive hørt.");
        s.MarkerAfholdt();

        s.Afslut();

        Assert.Equal(SamtaleStatus.Afsluttet, s.Status);
    }

    [Fact]
    public void Noter_med_perspektiv_saetter_inddragelse()
    {
        var s = NySamtale();
        Assert.False(s.HjemInddraget);
        Assert.False(s.ElevInddraget);

        s.TilføjNote(SamtaleFaseKode.InviterHjemmetsPerspektiv, Perspektiv.Hjem, "Sover dårligt om søndagen.");
        s.TilføjNote(SamtaleFaseKode.ElevensPerspektiv, Perspektiv.Elev, "Idræt er sværest.");

        Assert.True(s.HjemInddraget);
        Assert.True(s.ElevInddraget);
    }

    [Fact]
    public void Aflyst_samtale_kan_ikke_afholdes()
    {
        var s = NySamtale();
        s.Aflys("Sygdom");
        Assert.Throws<DomainException>(() => s.MarkerAfholdt());
    }
}
