using Skolehjemsamtale.Application.Abstractions;
using Skolehjemsamtale.Application.Dtos;
using Skolehjemsamtale.Application.Integration;
using Skolehjemsamtale.Application.Mapping;
using Skolehjemsamtale.Domain.Common;
using Skolehjemsamtale.Domain.Entities;
using Skolehjemsamtale.Domain.Framework;

namespace Skolehjemsamtale.Application.Services;

/// <summary>
/// Orkestrerer samtalelivscyklussen og udsender integrationsevents via outbox,
/// så værktøjet kan kobles til Techtree uden at kende modtagerens teknologi.
/// </summary>
public class SamtaleService
{
    private readonly ISamtaleRepository _samtaler;
    private readonly IElevRepository _elever;
    private readonly IObservationRepository _observationer;
    private readonly IUnitOfWork _uow;
    private readonly IIntegrationPublisher _publisher;
    private readonly IClock _clock;

    public SamtaleService(
        ISamtaleRepository samtaler, IElevRepository elever, IObservationRepository observationer,
        IUnitOfWork uow, IIntegrationPublisher publisher, IClock clock)
    {
        _samtaler = samtaler;
        _elever = elever;
        _observationer = observationer;
        _uow = uow;
        _publisher = publisher;
        _clock = clock;
    }

    public IReadOnlyList<SamtaleSkabelonDto> Skabeloner() =>
        DefaultSamtaleSkabeloner.Alle.Select(s => s.ToDto()).ToList();

    public IReadOnlyList<HjemmeSpoergsmaalDto> HjemmeSpoergeramme() =>
        Domain.Framework.HjemmeSpoergeramme.Alle.Select(s => s.ToDto()).ToList();

    public async Task<IReadOnlyList<SamtaleDto>> ListAsync(Guid? elevId = null, CancellationToken ct = default)
    {
        var list = await _samtaler.ListAsync(elevId, ct);
        return list.OrderByDescending(s => s.Dato).Select(s => s.ToDto(s.Elev?.Navn)).ToList();
    }

    public async Task<SamtaleDetaljerDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var s = await _samtaler.GetMedDetaljerAsync(id, ct);
        if (s is null) return null;
        return await ByggDetaljerAsync(s, ct);
    }

    public async Task<SamtaleDto> PlanlaegAsync(PlanlaegSamtaleRequest req, CancellationToken ct = default)
    {
        var elev = await _elever.GetAsync(req.ElevId, ct)
            ?? throw new DomainException("Elev findes ikke.");

        var samtale = Samtale.Planlæg(req.ElevId, req.Type, req.Dato, req.Formaal, req.SkabelonKode, req.Sted);

        foreach (var obsId in req.ObservationIds ?? Array.Empty<Guid>())
            samtale.KoblObservation(obsId);

        foreach (var d in req.Deltagere ?? Array.Empty<DeltagerRequest>())
            samtale.TilføjDeltager(d.Navn, d.Rolle, d.Deltog);

        await _samtaler.AddAsync(samtale, ct);
        await _uow.SaveChangesAsync(ct);

        await _publisher.PublicerAsync(new IntegrationEvent
        {
            EventType = IntegrationEventTypes.SamtalePlanlagt,
            Versionsnummer = 1,
            AggregateType = nameof(Samtale),
            AggregateId = samtale.Id,
            ElevId = samtale.ElevId,
            Payload = new SamtalePlanlagtV1(
                samtale.Id.ToString(), samtale.ElevId.ToString(), samtale.Type.ToString(),
                samtale.Dato.ToString("yyyy-MM-dd"), samtale.Formaal, samtale.SkabelonKode),
            Tidspunkt = _clock.UtcNow
        }, ct);

        return samtale.ToDto(elev.Navn);
    }

    public async Task<SamtaleDeltagerDto?> TilfoejDeltagerAsync(Guid id, DeltagerRequest req, CancellationToken ct = default)
    {
        var s = await LoadAsync(id, ct);
        var d = s.TilføjDeltager(req.Navn, req.Rolle, req.Deltog);
        await _uow.SaveChangesAsync(ct);
        return d.ToDto();
    }

    public async Task<SamtaleFaseNoteDto?> TilfoejNoteAsync(Guid id, TilfoejNoteRequest req, CancellationToken ct = default)
    {
        var s = await LoadAsync(id, ct);
        var note = s.TilføjNote(req.Fase, req.Perspektiv, req.Indhold, req.ObservationId);
        await _uow.SaveChangesAsync(ct);
        return note.ToDto();
    }

    public async Task<HandlingsaftaleDto?> TilfoejAftaleAsync(Guid id, TilfoejAftaleRequest req, CancellationToken ct = default)
    {
        var s = await LoadAsync(id, ct);
        var aftale = s.TilføjAftale(req.Beskrivelse, req.Ansvarlig, req.AnsvarligNavn, req.Frist, req.Evalueringsdato);
        await _uow.SaveChangesAsync(ct);

        await _publisher.PublicerAsync(new IntegrationEvent
        {
            EventType = IntegrationEventTypes.AftaleOprettet,
            Versionsnummer = 1,
            AggregateType = nameof(Handlingsaftale),
            AggregateId = aftale.Id,
            ElevId = s.ElevId,
            Payload = new AftaleOprettetV1(
                aftale.Id.ToString(), s.Id.ToString(), s.ElevId.ToString(), aftale.Beskrivelse,
                aftale.Ansvarlig.ToString(),
                aftale.Frist?.ToString("yyyy-MM-dd"), aftale.Evalueringsdato?.ToString("yyyy-MM-dd")),
            Tidspunkt = _clock.UtcNow
        }, ct);

        return aftale.ToDto();
    }

    public async Task<HjemmeIndsigtDto?> TilfoejHjemmeIndsigtAsync(Guid id, TilfoejHjemmeIndsigtRequest req, CancellationToken ct = default)
    {
        var s = await LoadAsync(id, ct);
        var indsigt = s.TilføjHjemmeIndsigt(req.Spoergsmaal, req.Svar, req.Kategori, req.BidragetAf);
        await _uow.SaveChangesAsync(ct);

        await _publisher.PublicerAsync(new IntegrationEvent
        {
            EventType = IntegrationEventTypes.HjemmeIndsigtRegistreret,
            Versionsnummer = 1,
            AggregateType = nameof(HjemmeIndsigt),
            AggregateId = indsigt.Id,
            ElevId = s.ElevId,
            Payload = new HjemmeIndsigtRegistreretV1(
                indsigt.Id.ToString(), s.ElevId.ToString(), s.Id.ToString(), indsigt.Kategori.ToString(),
                indsigt.Spoergsmaal, indsigt.Svar, indsigt.BidragetAf),
            Tidspunkt = _clock.UtcNow
        }, ct);

        return indsigt.ToDto();
    }

    public async Task EvaluerAftaleAsync(Guid samtaleId, Guid aftaleId, EvaluerAftaleRequest req, CancellationToken ct = default)
    {
        var s = await LoadAsync(samtaleId, ct);
        var aftale = s.Aftaler.FirstOrDefault(a => a.Id == aftaleId)
            ?? throw new DomainException("Aftalen findes ikke på samtalen.");
        aftale.Evaluer(req.Evaluering, req.Fortsaet);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task MarkerAfholdtAsync(Guid id, CancellationToken ct = default)
    {
        var s = await LoadAsync(id, ct);
        s.MarkerAfholdt();
        await _uow.SaveChangesAsync(ct);
    }

    public async Task AflysAsync(Guid id, AflysSamtaleRequest req, CancellationToken ct = default)
    {
        var s = await LoadAsync(id, ct);
        s.Aflys(req.Aarsag ?? string.Empty);
        await _uow.SaveChangesAsync(ct);
    }

    /// <summary>Afslutter samtalen og publicerer et afsluttende integrationsevent med kvalitetsscore.</summary>
    public async Task<SamtaleKvalitetDto> AfslutAsync(Guid id, CancellationToken ct = default)
    {
        var s = await LoadAsync(id, ct);
        var obs = await _observationer.ListByIdsAsync(
            s.Observationer.Select(o => o.ObservationId).ToList(), ct);
        var kvalitet = SamtaleKvalitetEvaluator.Evaluer(s, obs);

        s.Afslut();
        await _uow.SaveChangesAsync(ct);

        await _publisher.PublicerAsync(new IntegrationEvent
        {
            EventType = IntegrationEventTypes.SamtaleAfsluttet,
            Versionsnummer = 1,
            AggregateType = nameof(Samtale),
            AggregateId = s.Id,
            ElevId = s.ElevId,
            Payload = new SamtaleAfsluttetV1(
                s.Id.ToString(), s.ElevId.ToString(), s.Dato.ToString("yyyy-MM-dd"),
                s.Aftaler.Count, kvalitet.Score, s.HjemInddraget, s.ElevInddraget),
            Tidspunkt = _clock.UtcNow
        }, ct);

        return kvalitet;
    }

    private async Task<Samtale> LoadAsync(Guid id, CancellationToken ct)
    {
        var s = await _samtaler.GetMedDetaljerAsync(id, ct);
        return s ?? throw new DomainException("Samtalen findes ikke.");
    }

    private async Task<SamtaleDetaljerDto> ByggDetaljerAsync(Samtale s, CancellationToken ct)
    {
        var relaterede = await _observationer.ListByIdsAsync(s.Observationer.Select(o => o.ObservationId).ToList(), ct);
        var kvalitet = SamtaleKvalitetEvaluator.Evaluer(s, relaterede);
        var skabelon = DefaultSamtaleSkabeloner.Find(s.SkabelonKode).ToDto();

        return new SamtaleDetaljerDto(
            s.ToDto(s.Elev?.Navn),
            s.Deltagere.Select(d => d.ToDto()).ToList(),
            s.Noter.OrderBy(n => n.Fase).Select(n => n.ToDto()).ToList(),
            s.Aftaler.Select(a => a.ToDto()).ToList(),
            s.HjemmeIndsigter.Select(h => h.ToDto()).ToList(),
            relaterede.Select(o => o.ToDto()).ToList(),
            skabelon,
            kvalitet);
    }
}
