using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Skolehjemsamtale.Application.Dtos;
using Skolehjemsamtale.Application.Services;

namespace Skolehjemsamtale.Web.Pages.Elever;

public class IndexModel : PageModel
{
    private readonly ElevService _elever;
    public IndexModel(ElevService elever) => _elever = elever;

    public IReadOnlyList<ElevDto> Elever { get; private set; } = Array.Empty<ElevDto>();

    [BindProperty] public string Navn { get; set; } = string.Empty;
    [BindProperty] public string Klasse { get; set; } = string.Empty;

    public async Task OnGetAsync(CancellationToken ct) => Elever = await _elever.ListAsync(ct: ct);

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(Navn))
            await _elever.OpretAsync(new OpretElevRequest(Navn, Klasse), ct);
        return RedirectToPage();
    }
}
