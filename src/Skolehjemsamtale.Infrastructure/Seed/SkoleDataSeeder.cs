using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skolehjemsamtale.Infrastructure.Persistence;

namespace Skolehjemsamtale.Infrastructure.Seed;

/// <summary>
/// Sørger for at databasen findes og er seedet. Hvis en eksportfil fra det gamle
/// værktøj (samtale_vaerktoej_data.json) findes, importeres den; ellers lægges demodata ind.
/// </summary>
public class SkoleDataSeeder
{
    private readonly SkoleDbContext _db;
    private readonly ILogger<SkoleDataSeeder> _log;

    public SkoleDataSeeder(SkoleDbContext db, ILogger<SkoleDataSeeder> log)
    {
        _db = db;
        _log = log;
    }

    public async Task SeedAsync(string? seedFilePath = null, CancellationToken ct = default)
    {
        await _db.Database.EnsureCreatedAsync(ct);

        if (await _db.Elever.AnyAsync(ct))
        {
            _log.LogInformation("Databasen indeholder allerede data – springer seed over.");
            return;
        }

        SeedRoot root;
        if (!string.IsNullOrWhiteSpace(seedFilePath) && File.Exists(seedFilePath))
        {
            _log.LogInformation("Importerer eksisterende data fra {Path}.", seedFilePath);
            root = LegacyDataImporter.Parse(await File.ReadAllTextAsync(seedFilePath, ct));
        }
        else
        {
            _log.LogInformation("Ingen eksportfil fundet – indlæser demodata.");
            root = DemoData.Bygg();
        }

        var importer = new LegacyDataImporter(_db);
        var antal = await importer.ImporterAsync(root, ct);
        _log.LogInformation("Seed færdig: {Elever} elever, {Samtaler} samtaler.", root.Elever.Count, antal);
    }
}
