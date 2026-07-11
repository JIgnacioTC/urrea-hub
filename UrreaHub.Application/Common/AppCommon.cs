namespace UrreaHub.Application.Common;

public static class AppRoles
{
    public const string Colaborador = "Colaborador";
    public const string Jefe = "Jefe";
    public const string RhAdmin = "RhAdmin";
    public const string TiAdmin = "TiAdmin";
    public const string NominaAdmin = "NominaAdmin";

    public const string JefeOrRh = "Jefe,RhAdmin";
    public const string AdminOrTi = "RhAdmin,TiAdmin";
}

public class Result<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }

    public static Result<T> Ok(T data) => new() { Success = true, Data = data };
    public static Result<T> Fail(string error) => new() { Success = false, Error = error };
}

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int Total { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}

public interface ICurrentUser
{
    Guid ColaboradorId { get; }
    IReadOnlyList<string> Roles { get; }
    bool IsRhAdmin { get; }
    bool IsTiAdmin { get; }
    bool IsJefe { get; }
    bool IsNominaAdmin { get; }
}
