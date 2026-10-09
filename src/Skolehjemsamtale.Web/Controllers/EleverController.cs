using Microsoft.AspNetCore.Mvc;
using Skolehjemsamtale.Application.Dtos;
using Skolehjemsamtale.Application.Services;

namespace Skolehjemsamtale.Web.Controllers;

[ApiController]
[Route("api/v1/elever")]
public class EleverController : ControllerBase
{
    private readonly ElevService _elever;
    public EleverController(ElevService elever) => _elever = elever;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ElevDto>>> List([FromQuery] bool? aktiv, [FromQuery] string? klasse, CancellationToken ct)
        => Ok(await _elever.ListAsync(aktiv, klasse, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ElevDto>> Get(Guid id, CancellationToken ct)
    {
        var elev = await _elever.GetAsync(id, ct);
        return elev is null ? NotFound() : Ok(elev);
    }

    [HttpGet("{id:guid}/overblik")]
    public async Task<ActionResult<ElevOverblikDto>> Overblik(Guid id, CancellationToken ct)
    {
        var overblik = await _elever.GetOverblikAsync(id, ct);
        return overblik is null ? NotFound() : Ok(overblik);
    }

    [HttpPost]
    public async Task<ActionResult<ElevDto>> Opret([FromBody] OpretElevRequest req, CancellationToken ct)
    {
        var elev = await _elever.OpretAsync(req, ct);
        return CreatedAtAction(nameof(Get), new { id = elev.Id }, elev);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ElevDto>> Opdater(Guid id, [FromBody] OpdaterElevRequest req, CancellationToken ct)
    {
        var elev = await _elever.OpdaterAsync(id, req, ct);
        return elev is null ? NotFound() : Ok(elev);
    }
}
