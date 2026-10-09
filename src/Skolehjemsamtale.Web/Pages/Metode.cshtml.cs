using Microsoft.AspNetCore.Mvc.RazorPages;
using Skolehjemsamtale.Application.Dtos;
using Skolehjemsamtale.Application.Services;

namespace Skolehjemsamtale.Web.Pages;

public class MetodeModel : PageModel
{
    private readonly SamtaleService _samtaler;
    public MetodeModel(SamtaleService samtaler) => _samtaler = samtaler;

    public IReadOnlyList<SamtaleSkabelonDto> Skabeloner { get; private set; } = Array.Empty<SamtaleSkabelonDto>();
    public IReadOnlyList<HjemmeSpoergsmaalDto> Spoergeramme { get; private set; } = Array.Empty<HjemmeSpoergsmaalDto>();

    public void OnGet()
    {
        Skabeloner = _samtaler.Skabeloner();
        Spoergeramme = _samtaler.HjemmeSpoergeramme();
    }
}
