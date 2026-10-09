using Skolehjemsamtale.Domain.Common;

namespace Skolehjemsamtale.Domain.Entities;

/// <summary>Kobling mellem en samtale og de observationer den følger op på.</summary>
public class SamtaleObservation : Entity
{
    public Guid SamtaleId { get; set; }
    public Samtale? Samtale { get; set; }
    public Guid ObservationId { get; set; }
    public Observation? Observation { get; set; }

    private SamtaleObservation() { }

    public static SamtaleObservation Opret(Guid observationId) => new() { ObservationId = observationId };
}
