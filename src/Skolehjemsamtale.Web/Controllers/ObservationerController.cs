using Microsoft.AspNetCore.Mvc;
using Skolehjemsamtale.Application.Dtos;
using Skolehjemsamtale.Application.Services;

namespace Skolehjemsamtale.Web.Controllers;

[ApiController]
[Route("api/v1")]
public class ObservationerController : ControllerBase
{
    private readonly ObservationService _observationer;
    public ObservationerController(ObservationService observationer) => _observationer = observationer;

    [HttpGet("elever/{elevId:guid}/observationer")]
    public async Task<ActionResult<IReadOnlyList<ObservationDto>>> List(Guid elevId, CancellationToken ct)
        => Ok(await _observationer.ListForElevAsync(elevId, ct));

    [HttpPost("observationer")]
    public async Task<ActionResult<ObservationDto>> Registrer([FromBody] RegistrerObservationRequest req, CancellationToken ct)
    {
        var o = await _observationer.RegistrerAsync(req, ct);
        return CreatedAtAction(nameof(List), new { elevId = o.ElevId }, o);
    }

    [HttpPost("observationer/{id:guid}/del-med-hjem")]
    public async Task<ActionResult<ObservationDto>> DelMedHjem(Guid id, CancellationToken ct)
    {
        var o = await _observationer.MarkerDeltMedHjemAsync(id, ct);
        return o is null ? NotFound() : Ok(o);
    }
}
