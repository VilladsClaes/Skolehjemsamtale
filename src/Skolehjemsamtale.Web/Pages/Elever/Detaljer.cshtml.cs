using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Skolehjemsamtale.Application.Dtos;
using Skolehjemsamtale.Application.Services;
using Skolehjemsamtale.Domain.Enums;

namespace Skolehjemsamtale.Web.Pages.Elever;

public class DetaljerModel : PageModel
{
    private readonly ElevService _elever;
    private readonly ObservationService _observationer;
    public DetaljerModel(ElevService elever, ObservationService observationer)
    {
        _elever = elever;
        _observationer = observationer;
    }

    public ElevOverblikDto? Overblik { get; private set; }

    [BindProperty] public string Beskrivelse { get; set; } = string.Empty;
    [BindProperty] public string Kontekst { get; set; } = string.Empty;
    [BindProperty] public ObservationKategori Kategori { get; set; } = ObservationKategori.Trivsel;
    [BindProperty] public ObservationValens Valens { get; set; } = ObservationValens.Opmærksomhed;
    [BindProperty] public string? HvadHjalp { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct)
    {
        Overblik = await _elever.GetOverblikAsync(id, ct);
        return Overblik is null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostObservationAsync(Guid id, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(Beskrivelse))
        {
            await _observationer.RegistrerAsync(new RegistrerObservationRequest(
                id, DateOnly.FromDateTime(DateTime.UtcNow), Kategori, Valens,
                Beskrivelse, Kontekst, "Lærer (undertegnede)", HvadHjalp), ct);
        }
        return RedirectToPage(new { id });
    }
}
