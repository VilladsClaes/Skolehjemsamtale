using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Skolehjemsamtale.Infrastructure.Integration;

/// <summary>
/// HTTP-baseret gateway til Techtree. Sender et neutralt JSON-event med signatur,
/// så modtageren kan verificere afsenderen uafhængigt af teknologi.
/// </summary>
public class HttpTechtreeGateway : ITechtreeGateway
{
    private readonly HttpClient _http;
    private readonly TechtreeOptions _options;
    private readonly ILogger<HttpTechtreeGateway> _log;
    private readonly string? _secret;

    public HttpTechtreeGateway(HttpClient http, IOptions<TechtreeOptions> options, ILogger<HttpTechtreeGateway> log)
    {
        _http = http;
        _options = options.Value;
        _log = log;
        _secret = string.IsNullOrWhiteSpace(_options.SharedSecretName)
            ? null
            : Environment.GetEnvironmentVariable(_options.SharedSecretName);
    }

    public bool ErAktiveret => !string.IsNullOrWhiteSpace(_options.BaseUrl);

    public async Task LeverAsync(OutboxMessage message, CancellationToken ct = default)
    {
        if (!ErAktiveret)
            throw new InvalidOperationException("Techtree er ikke konfigureret (Techtree:BaseUrl mangler).");

        var url = _options.BaseUrl!.TrimEnd('/') + _options.EventsPath;
        var body = new
        {
            eventType = message.EventType,
            version = message.Versionsnummer,
            aggregateType = message.AggregateType,
            aggregateId = message.AggregateId.ToString(),
            elevId = message.ElevId?.ToString(),
            occurredAt = message.Oprettet,
            payload = message.Payload
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(body)
        };
        request.Headers.Add("X-Source", "Skolehjemsamtale");
        request.Headers.Add("X-Event-Id", message.Id.ToString());

        if (!string.IsNullOrEmpty(_secret))
        {
            var raw = JsonContent.Create(body);
            var bytes = await raw.ReadAsByteArrayAsync(ct);
            var sig = Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(_secret), bytes));
            request.Headers.Add("X-Signature", sig);
        }

        using var response = await _http.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var text = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"Techtree svarede {(int)response.StatusCode}: {text}");
        }
        _log.LogInformation("Event {EventType} ({Id}) leveret til Techtree.", message.EventType, message.Id);
    }
}
