using HouseXyloo.Api.Models;
using HouseXyloo.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HouseXyloo.Api.Controllers;

[ApiController]
[Route("api/house")]
public class HouseController : ControllerBase
{
    private readonly HouseStatusService _houseStatusService;

    public HouseController(HouseStatusService houseStatusService)
    {
        _houseStatusService = houseStatusService;
    }

    [HttpGet("status")]
    public async Task<ActionResult<HouseStatus>> GetStatus()
    {
        var status = await _houseStatusService.GetStatusAsync();

        return Ok(status);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("status")]
    public async Task<ActionResult<HouseStatus>> UpdateStatus(
        [FromBody] HouseStatus status)
    {
        var updatedStatus =
            await _houseStatusService.UpdateStatusAsync(status);

        return Ok(updatedStatus);
    }
}