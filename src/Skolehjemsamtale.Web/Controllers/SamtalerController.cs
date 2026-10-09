using Microsoft.AspNetCore.Mvc;
using Skolehjemsamtale.Application.Dtos;
using Skolehjemsamtale.Application.Services;

namespace Skolehjemsamtale.Web.Controllers;

[ApiController]
[Route("api/v1/samtaler")]
public class SamtalerController : ControllerBase
{
    private readonly SamtaleService _samtaler;
    public SamtalerController(SamtaleService samtaler) => _samtaler = samtaler;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SamtaleDto>>> List([FromQuery] Guid? elevId, CancellationToken ct)
        => Ok(await _samtaler.ListAsync(elevId, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SamtaleDetaljerDto>> Get(Guid id, CancellationToken ct)
    {
        var s = await _samtaler.GetAsync(id, ct);
        return s is null ? NotFound() : Ok(s);
    }

    [HttpPost]
    public async Task<ActionResult<SamtaleDto>> Planlaeg([FromBody] PlanlaegSamtaleRequest req, CancellationToken ct)
    {
        var s = await _samtaler.PlanlaegAsync(req, ct);
        return CreatedAtAction(nameof(Get), new { id = s.Id }, s);
    }

    [HttpPost("{id:guid}/deltagere")]
    public async Task<ActionResult<SamtaleDeltagerDto>> TilfoejDeltager(Guid id, [FromBody] DeltagerRequest req, CancellationToken ct)
        => Ok(await _samtaler.TilfoejDeltagerAsync(id, req, ct));

    [HttpPost("{id:guid}/noter")]
    public async Task<ActionResult<SamtaleFaseNoteDto>> TilfoejNote(Guid id, [FromBody] TilfoejNoteRequest req, CancellationToken ct)
        => Ok(await _samtaler.TilfoejNoteAsync(id, req, ct));

    [HttpPost("{id:guid}/aftaler")]
    public async Task<ActionResult<HandlingsaftaleDto>> TilfoejAftale(Guid id, [FromBody] TilfoejAftaleRequest req, CancellationToken ct)
        => Ok(await _samtaler.TilfoejAftaleAsync(id, req, ct));

    [HttpPut("{id:guid}/aftaler/{aftaleId:guid}")]
    public async Task<IActionResult> EvaluerAftale(Guid id, Guid aftaleId, [FromBody] EvaluerAftaleRequest req, CancellationToken ct)
    {
        await _samtaler.EvaluerAftaleAsync(id, aftaleId, req, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/hjemmeindsigter")]
    public async Task<ActionResult<HjemmeIndsigtDto>> TilfoejHjemmeIndsigt(Guid id, [FromBody] TilfoejHjemmeIndsigtRequest req, CancellationToken ct)
        => Ok(await _samtaler.TilfoejHjemmeIndsigtAsync(id, req, ct));

    [HttpPost("{id:guid}/afholdt")]
    public async Task<IActionResult> MarkerAfholdt(Guid id, CancellationToken ct)
    {
        await _samtaler.MarkerAfholdtAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/afslut")]
    public async Task<ActionResult<SamtaleKvalitetDto>> Afslut(Guid id, CancellationToken ct)
        => Ok(await _samtaler.AfslutAsync(id, ct));

    [HttpPost("{id:guid}/aflys")]
    public async Task<IActionResult> Aflys(Guid id, [FromBody] AflysSamtaleRequest req, CancellationToken ct)
    {
        await _samtaler.AflysAsync(id, req, ct);
        return NoContent();
    }
}
