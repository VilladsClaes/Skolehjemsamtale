using Skolehjemsamtale.Application.Abstractions;
using Skolehjemsamtale.Application.Dtos;
using Skolehjemsamtale.Application.Integration;
using Skolehjemsamtale.Application.Mapping;
using Skolehjemsamtale.Domain.Entities;

namespace Skolehjemsamtale.Application.Services;

/// <summary>
/// Registrering af observationer fra undervisningen. Hver observation publiceres til outbox,
/// så Techtree kan følge udviklingen uden at læse direkte i vores database.
/// </summary>
public class ObservationService
{
    private readonly IObservationRepository _observationer;
    private readonly IElevRepository _elever;
    private readonly IUnitOfWork _uow;
    private readonly IIntegrationPublisher _publisher;
    private readonly IClock _clock;

    public ObservationService(
        IObservationRepository observationer, IElevRepository elever,
        IUnitOfWork uow, IIntegrationPublisher publisher, IClock clock)
    {
        _observationer = observationer;
        _elever = elever;
        _uow = uow;
        _publisher = publisher;
        _clock = clock;
    }

    public async Task<IReadOnlyList<ObservationDto>> ListForElevAsync(Guid elevId, CancellationToken ct = default)
    {
        var list = await _observationer.ListForElevAsync(elevId, ct);
        return list.OrderByDescending(o => o.Dato).Select(o => o.ToDto()).ToList();
    }

    public async Task<ObservationDto> RegistrerAsync(RegistrerObservationRequest req, CancellationToken ct = default)
    {
        _ = await _elever.GetAsync(req.ElevId, ct)
            ?? throw new Domain.Common.DomainException("Elev findes ikke.");

        var o = Observation.Registrer(
            req.ElevId, req.Dato, req.Kategori, req.Valens,
            req.Beskrivelse, req.Kontekst, req.ObserveretAf, req.HvadHjalp);

        await _observationer.AddAsync(o, ct);
        await _uow.SaveChangesAsync(ct);

        await _publisher.PublicerAsync(new IntegrationEvent
        {
            EventType = IntegrationEventTypes.ObservationRegistreret,
            Versionsnummer = 1,
            AggregateType = nameof(Observation),
            AggregateId = o.Id,
            ElevId = o.ElevId,
            Payload = new ObservationRegistreretV1(
                o.ElevId.ToString(), o.Id.ToString(), o.Dato.ToString("yyyy-MM-dd"),
                o.Kategori.ToString(), o.Valens.ToString(), o.Beskrivelse, o.Kontekst, o.ObserveretAf),
            Tidspunkt = _clock.UtcNow
        }, ct);

        return o.ToDto();
    }

    public async Task<ObservationDto?> MarkerDeltMedHjemAsync(Guid id, CancellationToken ct = default)
    {
        var o = await _observationer.GetAsync(id, ct);
        if (o is null) return null;
        o.MarkerDeltMedHjem();
        await _uow.SaveChangesAsync(ct);
        return o.ToDto();
    }
}
