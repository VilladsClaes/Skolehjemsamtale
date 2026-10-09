using Microsoft.AspNetCore.Mvc;
using Skolehjemsamtale.Application.Dtos;
using Skolehjemsamtale.Application.Services;

namespace Skolehjemsamtale.Web.Controllers;

/// <summary>Metadata om den researchbaserede samtalestruktur, så klienter (og Techtree) kan vise den samme vejledning.</summary>
[ApiController]
[Route("api/v1/metode")]
public class MetodeController : ControllerBase
{
    private readonly SamtaleService _samtaler;
    public MetodeController(SamtaleService samtaler) => _samtaler = samtaler;

    [HttpGet("skabeloner")]
    public ActionResult<IReadOnlyList<SamtaleSkabelonDto>> Skabeloner() => Ok(_samtaler.Skabeloner());

    [HttpGet("hjemme-spoergsmaal")]
    public ActionResult<IReadOnlyList<HjemmeSpoergsmaalDto>> HjemmeSpoergsmaal() => Ok(_samtaler.HjemmeSpoergeramme());
}
