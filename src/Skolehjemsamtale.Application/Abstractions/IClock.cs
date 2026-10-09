namespace Skolehjemsamtale.Application.Abstractions;

/// <summary>Abstraktion over tid, så services kan testes deterministisk.</summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
    DateOnly Today { get; }
}

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
