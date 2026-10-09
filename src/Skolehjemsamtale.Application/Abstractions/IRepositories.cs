using Skolehjemsamtale.Domain.Entities;

namespace Skolehjemsamtale.Application.Abstractions;

public interface IElevRepository
{
    Task<Elev?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Elev?> GetMedAltAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Elev>> ListAsync(bool? aktiv = null, string? klasse = null, CancellationToken ct = default);
    Task AddAsync(Elev elev, CancellationToken ct = default);
    void Remove(Elev elev);
}

public interface IObservationRepository
{
    Task<Observation?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Observation>> ListForElevAsync(Guid elevId, CancellationToken ct = default);
    Task<IReadOnlyList<Observation>> ListByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct = default);
    Task AddAsync(Observation observation, CancellationToken ct = default);
}

public interface ISamtaleRepository
{
    Task<Samtale?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Samtale?> GetMedDetaljerAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Samtale>> ListAsync(Guid? elevId = null, CancellationToken ct = default);
    Task AddAsync(Samtale samtale, CancellationToken ct = default);
}

public interface IHjemmeIndsigtRepository
{
    Task<IReadOnlyList<HjemmeIndsigt>> ListForElevAsync(Guid elevId, CancellationToken ct = default);
}

/// <summary>Unit of work, så en forretningsoperation kan gemmes atomisk og udsende integrationsevents.</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default);
}
