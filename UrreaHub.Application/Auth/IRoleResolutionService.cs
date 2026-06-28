using UrreaHub.Domain.CoreRH;

namespace UrreaHub.Application.Auth;

public interface IRoleResolutionService
{
    Task<IReadOnlyList<string>> ResolveRolesAsync(Colaborador colaborador, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> ResolvePermissionsAsync(
        IReadOnlyList<string> roles,
        Guid colaboradorId,
        CancellationToken cancellationToken = default);
}
