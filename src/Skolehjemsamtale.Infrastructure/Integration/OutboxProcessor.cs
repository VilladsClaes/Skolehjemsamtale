using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skolehjemsamtale.Infrastructure.Persistence;

namespace Skolehjemsamtale.Infrastructure.Integration;

/// <summary>
/// Baggrundsprocessor der leverer outbox-events til Techtree med retry.
/// Kører kun, hvis integrationen er aktiveret. Postens payload sendes uændret,
/// så events kan genafspilles idempotent af modtageren (X-Event-Id).
/// </summary>
public class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TechtreeOptions _options;
    private readonly ILogger<OutboxProcessor> _log;

    public OutboxProcessor(IServiceScopeFactory scopeFactory, IOptions<TechtreeOptions> options, ILogger<OutboxProcessor> log)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.BaseUrl))
        {
            _log.LogInformation("Outbox-processor er ikke aktiv (Techtree ikke konfigureret eller deaktiveret).");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var antal = await BehandlBatchAsync(stoppingToken);
                if (antal == 0)
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Uventet fejl i outbox-processor.");
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }

    private async Task<int> BehandlBatchAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SkoleDbContext>();
        var gateway = scope.ServiceProvider.GetRequiredService<ITechtreeGateway>();

        var batch = await db.Outbox
            .Where(m => !m.Behandlet)
            .OrderBy(m => m.OprettetUtc)
            .Take(_options.BatchSize)
            .ToListAsync(ct);

        foreach (var msg in batch)
        {
            try
            {
                await gateway.LeverAsync(msg, ct);
                msg.Behandlet = true;
                msg.BehandletTid = DateTimeOffset.UtcNow;
                msg.Fejl = null;
            }
            catch (Exception ex)
            {
                msg.Forsøg++;
                msg.Fejl = ex.Message.Length > 1000 ? ex.Message[..1000] : ex.Message;
                if (msg.Forsøg >= _options.MaksForsoeg)
                {
                    msg.Behandlet = true; // giv op, men bevar posten til manuel genafspilning
                    msg.BehandletTid = DateTimeOffset.UtcNow;
                    _log.LogError(ex, "Giver op på event {EventType} ({Id}) efter {Forsoeg} forsøg.", msg.EventType, msg.Id, msg.Forsøg);
                }
                else
                {
                    _log.LogWarning(ex, "Levering af {EventType} ({Id}) fejlede, forsøg {Forsoeg}.", msg.EventType, msg.Id, msg.Forsøg);
                }
            }
        }

        if (batch.Count > 0)
            await db.SaveChangesAsync(ct);

        return batch.Count;
    }
}
