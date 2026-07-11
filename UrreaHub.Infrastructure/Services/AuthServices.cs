using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UrreaHub.Application.Auth;
using UrreaHub.Application.Common;

namespace UrreaHub.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> settings) => _settings = settings.Value;

    public string GenerateToken(Guid colaboradorId, string nombre, IReadOnlyList<string> roles, IReadOnlyList<string>? permissions = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, colaboradorId.ToString()),
            new(ClaimTypes.Name, nombre)
        };
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));
        foreach (var permission in permissions ?? Array.Empty<string>())
            claims.Add(new Claim(AppPermissions.ClaimType, permission));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            _settings.Issuer,
            _settings.Audience,
            claims,
            expires: DateTime.UtcNow.AddHours(_settings.ExpirationHours),
            signingCredentials: creds);

        return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public Guid ColaboradorId
    {
        get
        {
            var id = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
        }
    }

    public IReadOnlyList<string> Roles =>
        _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        ?? new List<string>();

    public bool IsRhAdmin => Roles.Contains(AppRoles.RhAdmin);
    public bool IsTiAdmin => Roles.Contains(AppRoles.TiAdmin) || IsRhAdmin;
    public bool IsJefe => Roles.Contains(AppRoles.Jefe) || IsRhAdmin;
    public bool IsNominaAdmin => Roles.Contains(AppRoles.NominaAdmin) || IsRhAdmin;
}

public class AuthService : IAuthService
{
    private readonly Persistence.UrreaHubDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRoleResolutionService _roleResolution;

    public AuthService(
        Persistence.UrreaHubDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IRoleResolutionService roleResolution)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _roleResolution = roleResolution;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var identificador = request.Identificador.Trim();
        var colaborador = await _context.Colaboradores
            .Include(c => c.CuentaAcceso)
            .Include(c => c.DatosSensibles)
            .FirstOrDefaultAsync(c => c.IsActive && (
                c.NumeroEmpleado == identificador ||
                c.Email == identificador ||
                (c.DatosSensibles != null && c.DatosSensibles.Rfc == identificador)), cancellationToken);

        if (colaborador?.CuentaAcceso is null)
            return null;

        if (!_passwordHasher.Verify(request.Password, colaborador.CuentaAcceso.PasswordHash))
            return null;

        var roles = await _roleResolution.ResolveRolesAsync(colaborador, cancellationToken);
        var permissions = await _roleResolution.ResolvePermissionsAsync(roles, colaborador.Id, cancellationToken);
        var nombre = $"{colaborador.Nombre} {colaborador.ApellidoPaterno}".Trim();

        colaborador.CuentaAcceso.UltimoAcceso = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResponseDto
        {
            Token = _jwtTokenService.GenerateToken(colaborador.Id, nombre, roles, permissions),
            ColaboradorId = colaborador.Id,
            NombreCompleto = nombre,
            NumeroEmpleado = colaborador.NumeroEmpleado,
            Roles = roles,
            DebeCambiarPassword = colaborador.CuentaAcceso.DebeCambiarPassword
        };
    }
}
