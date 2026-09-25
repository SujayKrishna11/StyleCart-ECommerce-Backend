using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StyleCart.Application.DTOs.Admin;
using StyleCart.Application.Interfaces;

namespace StyleCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminUserService _adminUserService;

    public AdminController(IAdminUserService adminUserService)
    {
        _adminUserService = adminUserService;
    }

    [HttpPost("promote-to-admin")]
    public async Task<ActionResult<PromoteUserResponse>> PromoteToAdmin(
        PromoteUserRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _adminUserService.PromoteToAdminAsync(
            request,
            cancellationToken);

        return response is null
            ? NotFound(new { message = "No user was found with this email address." })
            : Ok(response);
    }
    [HttpGet("users")]
    public async Task<ActionResult<IReadOnlyList<AdminUserResponse>>> GetAllUsers(
    CancellationToken cancellationToken)
    {
        var users = await _adminUserService.GetAllUsersAsync(cancellationToken);

        return Ok(users);
    }
}