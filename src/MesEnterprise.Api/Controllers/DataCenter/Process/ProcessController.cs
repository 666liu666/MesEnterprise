using MediatR;
using MesEnterprise.Api.Controllers;
using MesEnterprise.Application.Features.DataCenter.Processes;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Api.Controllers.DataCenter.Process;

[Route("api/v{version:apiVersion}/data-center/processes")]
[ApiVersion("1.0")]
public class ProcessController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProcesses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(new GetProcessesQuery(pageNumber, pageSize), cancellationToken);
        return Ok(response);
    }
}
