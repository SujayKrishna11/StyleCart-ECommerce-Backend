using Microsoft.AspNetCore.Identity;
using StyleCart.Application.DTOs.Admin;
using StyleCart.Application.Interfaces;
using StyleCart.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace StyleCart.Infrastructure.Services;

public class AdminUserService : IAdminUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminUserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<PromoteUserResponse?> PromoteToAdminAsync(
        PromoteUserRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new InvalidOperationException("Email is required.");
        }

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return null;
        }

        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return new PromoteUserResponse
            {
                Email = user.Email!,
                Message = "This user is already an Admin."
            };
        }

        var result = await _userManager.AddToRoleAsync(user, "Admin");

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Unable to promote the user to Admin. {errors}");
        }

        return new PromoteUserResponse
        {
            Email = user.Email!,
            Message = "User promoted to Admin successfully."
        };
    }
    public async Task<IReadOnlyList<AdminUserResponse>> GetAllUsersAsync(
    CancellationToken cancellationToken = default)
    {
        var users = await _userManager.Users
            .OrderBy(user => user.Email)
            .ToListAsync(cancellationToken);

        var responses = new List<AdminUserResponse>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            responses.Add(new AdminUserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList()
            });
        }

        return responses;
    }
}