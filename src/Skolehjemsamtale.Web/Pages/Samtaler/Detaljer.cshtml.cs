using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Skolehjemsamtale.Application.Dtos;
using Skolehjemsamtale.Application.Services;
using Skolehjemsamtale.Domain.Enums;

namespace Skolehjemsamtale.Web.Pages.Samtaler;

public class DetaljerModel : PageModel
{
    private readonly SamtaleService _samtaler;
    public DetaljerModel(SamtaleService samtaler) => _samtaler = samtaler;

    public SamtaleDetaljerDto? Samtale { get; private set; }
    public IReadOnlyList<HjemmeSpoergsmaalDto> Spoergeramme { get; private set; } = Array.Empty<HjemmeSpoergsmaalDto>();

    [BindProperty] public SamtaleFaseKode NoteFase { get; set; } = SamtaleFaseKode.Aabning;
    [BindProperty] public Perspektiv NotePerspektiv { get; set; } = Perspektiv.Skole;
    [BindProperty] public string NoteIndhold { get; set; } = string.Empty;

    [BindProperty] public string AftaleBeskrivelse { get; set; } = string.Empty;
    [BindProperty] public AftaleAnsvar AftaleAnsvarlig { get; set; } = AftaleAnsvar.Faelles;
    [BindProperty] public DateOnly? AftaleFrist { get; set; }
    [BindProperty] public DateOnly? AftaleEvaluering { get; set; }

    [BindProperty] public string HjemmeSpoergsmaal { get; set; } = string.Empty;
    [BindProperty] public string HjemmeSvar { get; set; } = string.Empty;
    [BindProperty] public ObservationKategori HjemmeKategori { get; set; } = ObservationKategori.Trivsel;
    [BindProperty] public string HjemmeBidragetAf { get; set; } = string.Empty;

    [BindProperty] public string DeltagerNavn { get; set; } = string.Empty;
    [BindProperty] public SamtaleRolle DeltagerRolle { get; set; } = SamtaleRolle.Foraelder;

    [BindProperty] public string? Aflysningsaarsag { get; set; }
    [BindProperty] public string? EvalueringsTekst { get; set; }

    public string? Fejl { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct)
    {
        Samtale = await _samtaler.GetAsync(id, ct);
        if (Samtale is null) return NotFound();
        Spoergeramme = _samtaler.HjemmeSpoergeramme();
        return Page();
    }

    private async Task<IActionResult> Genindlæs(Guid id, CancellationToken ct)
    {
        Samtale = await _samtaler.GetAsync(id, ct);
        return Samtale is null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostNoteAsync(Guid id, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(NoteIndhold))
            await _samtaler.TilfoejNoteAsync(id, new TilfoejNoteRequest(NoteFase, NotePerspektiv, NoteIndhold), ct);
        return await Genindlæs(id, ct);
    }

    public async Task<IActionResult> OnPostAftaleAsync(Guid id, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(AftaleBeskrivelse))
            await _samtaler.TilfoejAftaleAsync(id, new TilfoejAftaleRequest(
                AftaleBeskrivelse, AftaleAnsvarlig, null, AftaleFrist, AftaleEvaluering), ct);
        return await Genindlæs(id, ct);
    }

    public async Task<IActionResult> OnPostHjemmeAsync(Guid id, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(HjemmeSvar))
            await _samtaler.TilfoejHjemmeIndsigtAsync(id, new TilfoejHjemmeIndsigtRequest(
                HjemmeSpoergsmaal, HjemmeSvar, HjemmeKategori, HjemmeBidragetAf), ct);
        return await Genindlæs(id, ct);
    }

    public async Task<IActionResult> OnPostDeltagerAsync(Guid id, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(DeltagerNavn))
            await _samtaler.TilfoejDeltagerAsync(id, new DeltagerRequest(DeltagerNavn, DeltagerRolle), ct);
        return await Genindlæs(id, ct);
    }

    public async Task<IActionResult> OnPostAfholdtAsync(Guid id, CancellationToken ct)
    {
        await _samtaler.MarkerAfholdtAsync(id, ct);
        return await Genindlæs(id, ct);
    }

    public async Task<IActionResult> OnPostAfslutAsync(Guid id, CancellationToken ct)
    {
        try { await _samtaler.AfslutAsync(id, ct); }
        catch (Domain.Common.DomainException ex) { Fejl = ex.Message; }
        return await Genindlæs(id, ct);
    }

    public async Task<IActionResult> OnPostAflysAsync(Guid id, CancellationToken ct)
    {
        await _samtaler.AflysAsync(id, new AflysSamtaleRequest(Aflysningsaarsag), ct);
        return await Genindlæs(id, ct);
    }
}
