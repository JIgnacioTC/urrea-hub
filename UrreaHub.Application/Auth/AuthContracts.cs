namespace UrreaHub.Application.Auth;

public class LoginRequestDto
{
    public string Identificador { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public Guid ColaboradorId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string NumeroEmpleado { get; set; } = string.Empty;
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public bool DebeCambiarPassword { get; set; }
}

public class JwtSettings
{
    public const string SectionName = "Jwt";
    public string Secret { get; set; } = "UrreaHub-Dev-Secret-Key-Min32Chars!!";
    public string Issuer { get; set; } = "UrreaHub";
    public string Audience { get; set; } = "UrreaHub";
    public int ExpirationHours { get; set; } = 12;
}

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}

public interface IJwtTokenService
{
    string GenerateToken(Guid colaboradorId, string nombre, IReadOnlyList<string> roles, IReadOnlyList<string>? permissions = null);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
