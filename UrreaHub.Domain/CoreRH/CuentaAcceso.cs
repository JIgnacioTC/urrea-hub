using UrreaHub.Domain.Common;

namespace UrreaHub.Domain.CoreRH;

public class CuentaAcceso : BaseEntity
{
    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime? UltimoAcceso { get; set; }
    public bool DebeCambiarPassword { get; set; }
    public bool EsRhAdmin { get; set; }
    public bool EsTiAdmin { get; set; }
}
