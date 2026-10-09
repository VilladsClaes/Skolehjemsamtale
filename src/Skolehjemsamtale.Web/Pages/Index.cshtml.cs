using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Skolehjemsamtale.Infrastructure.Persistence;

namespace Skolehjemsamtale.Web.Pages;

public class IndexModel : PageModel
{
    private readonly SkoleDbContext _db;
    public IndexModel(SkoleDbContext db) => _db = db;

    public int AntalElever { get; private set; }
    public int AntalPlanlagte { get; private set; }
    public int AntalAabneAftaler { get; private set; }
    public int AntalHjemmeIndsigter { get; private set; }
    public List<SamtaleRaekke> Kommende { get; private set; } = new();

    public record SamtaleRaekke(Guid Id, string Elev, string Type, string Status, DateOnly Dato, string Formaal);

    public async Task OnGetAsync(CancellationToken ct)
    {
        AntalElever = await _db.Elever.CountAsync(e => e.Aktiv, ct);
        AntalPlanlagte = await _db.Samtaler.CountAsync(s => s.Status == Domain.Enums.SamtaleStatus.Planlagt, ct);
        AntalAabneAftaler = await _db.Handlingsaftaler.CountAsync(a => a.Status != Domain.Enums.AftaleStatus.Afsluttet, ct);
        AntalHjemmeIndsigter = await _db.HjemmeIndsigter.CountAsync(ct);

        Kommende = await _db.Samtaler
            .Include(s => s.Elev)
            .OrderByDescending(s => s.Dato)
            .Take(8)
            .Select(s => new SamtaleRaekke(s.Id, s.Elev!.Navn, s.Type.ToString(), s.Status.ToString(), s.Dato, s.Formaal))
            .ToListAsync(ct);
    }
}
