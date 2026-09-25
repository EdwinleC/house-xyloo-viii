using HouseXyloo.Api.Models.Auth;
using HouseXyloo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HouseXyloo.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (response is null)
        {
            return Unauthorized();
        }

        return Ok(response);
    }
}