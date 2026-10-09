using Skolehjemsamtale.Application.Abstractions;
using Skolehjemsamtale.Application.Dtos;
using Skolehjemsamtale.Application.Mapping;
using Skolehjemsamtale.Domain.Entities;

namespace Skolehjemsamtale.Application.Services;

public class ElevService
{
    private readonly IElevRepository _elever;
    private readonly IUnitOfWork _uow;

    public ElevService(IElevRepository elever, IUnitOfWork uow)
    {
        _elever = elever;
        _uow = uow;
    }

    public async Task<IReadOnlyList<ElevDto>> ListAsync(bool? aktiv = null, string? klasse = null, CancellationToken ct = default)
    {
        var elever = await _elever.ListAsync(aktiv, klasse, ct);
        return elever.Select(e => e.ToDto(e.Observationer.Count, e.Samtaler.Count)).ToList();
    }

    public async Task<ElevDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _elever.GetAsync(id, ct);
        return e?.ToDto(e.Observationer.Count, e.Samtaler.Count);
    }

    public async Task<ElevOverblikDto?> GetOverblikAsync(Guid id, CancellationToken ct = default)
    {
        var elev = await _elever.GetMedAltAsync(id, ct);
        if (elev is null) return null;

        var aabneAftaler = elev.Samtaler.SelectMany(s => s.Aftaler)
            .Count(a => a.Status == Domain.Enums.AftaleStatus.Aktiv || a.Status == Domain.Enums.AftaleStatus.Foreslaaet);

        return new ElevOverblikDto(
            elev.ToDto(elev.Observationer.Count, elev.Samtaler.Count),
            elev.Samtaler.OrderByDescending(s => s.Dato).Select(s => s.ToDto(elev.Navn)).ToList(),
            elev.Observationer.OrderByDescending(o => o.Dato).Take(15).Select(o => o.ToDto()).ToList(),
            elev.HjemmeIndsigter.OrderByDescending(h => h.Oprettet).Select(h => h.ToDto()).ToList(),
            aabneAftaler);
    }

    public async Task<ElevDto> OpretAsync(OpretElevRequest req, CancellationToken ct = default)
    {
        var elev = Elev.Opret(req.Navn, req.Klasse, req.Foedselsdato);
        await _elever.AddAsync(elev, ct);
        await _uow.SaveChangesAsync(ct);
        return elev.ToDto();
    }

    public async Task<ElevDto?> OpdaterAsync(Guid id, OpdaterElevRequest req, CancellationToken ct = default)
    {
        var elev = await _elever.GetAsync(id, ct);
        if (elev is null) return null;
        elev.Opdater(req.Navn, req.Klasse, req.Aktiv);
        await _uow.SaveChangesAsync(ct);
        return elev.ToDto(elev.Observationer.Count, elev.Samtaler.Count);
    }
}
