using StyleCart.Application.DTOs.Admin;

namespace StyleCart.Application.Interfaces;

public interface IAdminUserService
{
    Task<PromoteUserResponse?> PromoteToAdminAsync(
        PromoteUserRequest request,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminUserResponse>> GetAllUsersAsync(
    CancellationToken cancellationToken = default);
}