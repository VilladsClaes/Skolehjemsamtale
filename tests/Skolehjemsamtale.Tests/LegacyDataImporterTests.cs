using Microsoft.EntityFrameworkCore;
using Skolehjemsamtale.Domain.Enums;
using Skolehjemsamtale.Infrastructure.Seed;
using Xunit;

namespace Skolehjemsamtale.Tests;

public class LegacyDataImporterTests
{
    private const string Json = """
    {
      "version": 1,
      "elever": [
        {
          "navn": "Emil",
          "klasse": "6C",
          "observationer": [
            { "dato": "2026-09-01", "kategori": "Deltagelse", "valens": "Opmærksomhed", "beskrivelse": "Sen ankomst", "kontekst": "Matematik", "observeretAf": "Lærer" },
            { "dato": "2026-09-02", "kategori": "Faglig", "valens": "Styrke", "beskrivelse": "Godt oplæg", "kontekst": "Dansk", "observeretAf": "Lærer" }
          ],
          "samtaler": [
            {
              "type": "SkoleHjemSamtale",
              "dato": "2026-09-10",
              "formaal": "Fælles forståelse",
              "skabelonKode": "skole-hjem-grund",
              "status": "Planlagt",
              "noter": [ { "fase": "Aabning", "perspektiv": "Hjem", "indhold": "Mor fortæller om morgener." } ],
              "hjemmeIndsigter": [ { "spoergsmaal": "Søvn?", "svar": "Sover sent", "kategori": "Trivsel", "bidragetAf": "Mor" } ]
            }
          ]
        }
      ]
    }
    """;

    [Fact]
    public async Task Importerer_elever_observationer_og_samtaler()
    {
        using var f = new TestDb();
        var root = LegacyDataImporter.Parse(Json);
        var importer = new LegacyDataImporter(f.Context);

        var antal = await importer.ImporterAsync(root);

        Assert.Equal(1, antal);
        Assert.Equal(1, await f.Context.Elever.CountAsync());
        Assert.Equal(2, await f.Context.Observationer.CountAsync());
        Assert.Equal(1, await f.Context.Samtaler.CountAsync());
        Assert.Equal(1, await f.Context.HjemmeIndsigter.CountAsync());
        Assert.True(await f.Context.Samtaler.AnyAsync(s => s.HjemInddraget));
    }

    [Fact]
    public async Task Demo_data_kan_importeres_uden_fejl()
    {
        using var f = new TestDb();
        var importer = new LegacyDataImporter(f.Context);

        await importer.ImporterAsync(DemoData.Bygg());

        Assert.True(await f.Context.Elever.CountAsync() >= 2);
        Assert.True(await f.Context.Samtaler.CountAsync() >= 2);
        Assert.True(await f.Context.HjemmeIndsigter.AnyAsync());
    }
}
