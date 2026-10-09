using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Skolehjemsamtale.Application.Dtos;
using Skolehjemsamtale.Application.Services;
using Skolehjemsamtale.Domain.Enums;

namespace Skolehjemsamtale.Web.Pages.Samtaler;

public class IndexModel : PageModel
{
    private readonly SamtaleService _samtaler;
    private readonly ElevService _elever;
    public IndexModel(SamtaleService samtaler, ElevService elever)
    {
        _samtaler = samtaler;
        _elever = elever;
    }

    public IReadOnlyList<SamtaleDto> Samtaler { get; private set; } = Array.Empty<SamtaleDto>();
    public IReadOnlyList<ElevDto> Elever { get; private set; } = Array.Empty<ElevDto>();
    public IReadOnlyList<SamtaleSkabelonDto> Skabeloner { get; private set; } = Array.Empty<SamtaleSkabelonDto>();

    [BindProperty] public Guid ElevId { get; set; }
    [BindProperty] public SamtaleType Type { get; set; } = SamtaleType.SkoleHjemSamtale;
    [BindProperty] public DateOnly Dato { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    [BindProperty] public string Formaal { get; set; } = string.Empty;
    [BindProperty] public string SkabelonKode { get; set; } = "skole-hjem-grund";
    [BindProperty] public string? Sted { get; set; }

    public async Task OnGetAsync(CancellationToken ct)
    {
        Samtaler = await _samtaler.ListAsync(ct: ct);
        Elever = await _elever.ListAsync(aktiv: true, ct: ct);
        Skabeloner = _samtaler.Skabeloner();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (ElevId != Guid.Empty && !string.IsNullOrWhiteSpace(Formaal))
        {
            var s = await _samtaler.PlanlaegAsync(new PlanlaegSamtaleRequest(
                ElevId, Type, Dato, Formaal, SkabelonKode, Sted), ct);
            return RedirectToPage("/Samtaler/Detaljer", new { id = s.Id });
        }
        return RedirectToPage();
    }
}
