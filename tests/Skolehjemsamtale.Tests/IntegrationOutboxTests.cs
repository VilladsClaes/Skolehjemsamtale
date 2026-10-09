using Microsoft.EntityFrameworkCore;
using Skolehjemsamtale.Application.Abstractions;
using Skolehjemsamtale.Application.Dtos;
using Skolehjemsamtale.Application.Integration;
using Skolehjemsamtale.Application.Services;
using Skolehjemsamtale.Domain.Entities;
using Skolehjemsamtale.Domain.Enums;
using Skolehjemsamtale.Infrastructure.Integration;
using Skolehjemsamtale.Infrastructure.Persistence;
using Skolehjemsamtale.Infrastructure.Persistence.Repositories;
using Xunit;

namespace Skolehjemsamtale.Tests;

public class IntegrationOutboxTests
{
    private static SamtaleService Bygg(SkoleDbContext db) =>
        new(new SamtaleRepository(db), new ElevRepository(db), new ObservationRepository(db),
            new EfUnitOfWork(db), new EfIntegrationPublisher(db), new SystemClock());

    [Fact]
    public async Task Observation_publiceres_til_outbox()
    {
        using var f = new TestDb();
        var elev = Elev.Opret("Test Elev", "7A");
        f.Context.Elever.Add(elev);
        await f.Context.SaveChangesAsync();

        var svc = new ObservationService(new ObservationRepository(f.Context), new ElevRepository(f.Context),
            new EfUnitOfWork(f.Context), new EfIntegrationPublisher(f.Context), new SystemClock());

        await svc.RegistrerAsync(new RegistrerObservationRequest(elev.Id, DateOnly.FromDateTime(DateTime.UtcNow),
            ObservationKategori.Faglig, ObservationValens.Styrke, "Godt oplæg", "Dansk", "Lærer"));

        var msg = await f.Context.Outbox.SingleAsync();
        Assert.Equal(IntegrationEventTypes.ObservationRegistreret, msg.EventType);
        Assert.Contains("Godt oplæg", msg.Payload);
    }

    [Fact]
    public async Task Planlaegning_skriver_event_til_outbox()
    {
        using var f = new TestDb();
        var elev = Elev.Opret("Test Elev", "7A");
        f.Context.Elever.Add(elev);
        await f.Context.SaveChangesAsync();

        var svc = Bygg(f.Context);
        await svc.PlanlaegAsync(new PlanlaegSamtaleRequest(
            elev.Id, SamtaleType.SkoleHjemSamtale, DateOnly.FromDateTime(DateTime.UtcNow),
            "Fælles formål", "skole-hjem-grund"));

        var msg = await f.Context.Outbox.SingleAsync();
        Assert.Equal(IntegrationEventTypes.SamtalePlanlagt, msg.EventType);
        Assert.Equal(1, msg.Versionsnummer);
        Assert.False(msg.Behandlet);
        Assert.Contains("Fælles formål", msg.Payload);
    }

    [Fact]
    public async Task Afslutning_publicerer_event_med_kvalitetsscore()
    {
        using var f = new TestDb();
        var elev = Elev.Opret("Test Elev", "7A");
        f.Context.Elever.Add(elev);
        await f.Context.SaveChangesAsync();

        var svc = Bygg(f.Context);
        var s = await svc.PlanlaegAsync(new PlanlaegSamtaleRequest(
            elev.Id, SamtaleType.Elevsamtale, DateOnly.FromDateTime(DateTime.UtcNow), "Formål"));

        await svc.TilfoejAftaleAsync(s.Id, new TilfoejAftaleRequest("Lille skridt", AftaleAnsvar.Faelles,
            Evalueringsdato: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14))));
        await svc.TilfoejNoteAsync(s.Id, new TilfoejNoteRequest(SamtaleFaseKode.StemmeTjek, Perspektiv.Skole, "Oplevede sig hørt."));
        await svc.MarkerAfholdtAsync(s.Id);

        var kvalitet = await svc.AfslutAsync(s.Id);

        Assert.True(kvalitet.Score > 0);
        var events = await f.Context.Outbox.Select(m => m.EventType).ToListAsync();
        Assert.Contains(IntegrationEventTypes.SamtaleAfsluttet, events);
        Assert.Contains(IntegrationEventTypes.SamtalePlanlagt, events);
    }

    [Fact]
    public async Task Outbox_kan_sorteres_og_filtreres_i_databasen()
    {
        using var f = new TestDb();
        var elev = Elev.Opret("Test Elev", "7A");
        f.Context.Elever.Add(elev);
        await f.Context.SaveChangesAsync();

        var svc = Bygg(f.Context);
        await svc.PlanlaegAsync(new PlanlaegSamtaleRequest(
            elev.Id, SamtaleType.SkoleHjemSamtale, DateOnly.FromDateTime(DateTime.UtcNow), "Et"));
        await svc.PlanlaegAsync(new PlanlaegSamtaleRequest(
            elev.Id, SamtaleType.SkoleHjemSamtale, DateOnly.FromDateTime(DateTime.UtcNow), "To"));

        // Samme forespørgsel som OutboxProcessor/IntegrationController bruger.
        var ventende = await f.Context.Outbox
            .Where(m => !m.Behandlet)
            .OrderBy(m => m.OprettetUtc)
            .ToListAsync();

        Assert.Equal(2, ventende.Count);
        Assert.True(ventende[0].OprettetUtc <= ventende[1].OprettetUtc);
    }

    [Fact]
    public async Task Hjemmeindsigt_fra_hjemmet_publiceres_og_gemmes()
    {
        using var f = new TestDb();
        var elev = Elev.Opret("Test Elev", "7A");
        f.Context.Elever.Add(elev);
        await f.Context.SaveChangesAsync();

        var svc = Bygg(f.Context);
        var s = await svc.PlanlaegAsync(new PlanlaegSamtaleRequest(
            elev.Id, SamtaleType.SkoleHjemSamtale, DateOnly.FromDateTime(DateTime.UtcNow), "Formål"));

        await svc.TilfoejHjemmeIndsigtAsync(s.Id, new TilfoejHjemmeIndsigtRequest(
            "Hvordan er morgenerne?", "Sover dårligt om søndagen", ObservationKategori.Trivsel, "Mor"));

        Assert.Equal(1, await f.Context.HjemmeIndsigter.CountAsync());
        Assert.Contains(await f.Context.Outbox.Select(m => m.EventType).ToListAsync(),
            e => e == IntegrationEventTypes.HjemmeIndsigtRegistreret);
    }
}
