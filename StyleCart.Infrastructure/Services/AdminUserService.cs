using Microsoft.AspNetCore.Identity;
using StyleCart.Application.DTOs.Admin;
using StyleCart.Application.Interfaces;
using StyleCart.Infrastructure.Identity;

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
}