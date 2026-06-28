namespace UrreaHub.Application.Notificaciones;

public record NotificationRequest(
    string Evento,
    string DestinatarioEmail,
    string Asunto,
    string Contenido,
    Guid? EntidadId = null);

public class AzureAdSettings
{
    public const string SectionName = "AzureAd";
    public string? TenantId { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? TeamsWebhookUrl { get; set; }
    public string FromEmail { get; set; } = "noreply@urrea.com";
}

public interface INotificationSender
{
    Task SendAsync(NotificationRequest request, CancellationToken cancellationToken = default);
}

public interface IAuditService
{
    Task LogEventoAsync(string modulo, string accion, string entidad, Guid entidadId, string? usuario, string? detalle, CancellationToken cancellationToken = default);
    Task LogCambioEstadoAsync(string entidad, Guid entidadId, string anterior, string nuevo, string? usuario, string? motivo, CancellationToken cancellationToken = default);
}
