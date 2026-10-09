using Skolehjemsamtale.Domain.Common;
using Skolehjemsamtale.Domain.Entities;
using Skolehjemsamtale.Domain.Enums;
using Xunit;

namespace Skolehjemsamtale.Tests;

public class ObservationTests
{
    [Fact]
    public void Registrering_kraever_beskrivelse_og_observatoer()
    {
        Assert.Throws<DomainException>(() => Observation.Registrer(
            Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), ObservationKategori.Trivsel,
            ObservationValens.Opmærksomhed, "  ", "Matematik", "Lærer"));

        Assert.Throws<DomainException>(() => Observation.Registrer(
            Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), ObservationKategori.Trivsel,
            ObservationValens.Opmærksomhed, "Sen ankomst", "Matematik", " "));
    }

    [Fact]
    public void Registrering_kan_markeres_delt_med_hjem()
    {
        var o = Observation.Registrer(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow),
            ObservationKategori.Faglig, ObservationValens.Styrke, "Hjalp en klassekammerat", "Naturfag", "Lærer");
        Assert.False(o.DeltMedHjem);
        o.MarkerDeltMedHjem();
        Assert.True(o.DeltMedHjem);
    }
}
