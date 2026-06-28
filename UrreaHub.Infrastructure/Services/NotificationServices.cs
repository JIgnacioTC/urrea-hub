using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Azure.Identity;
using UrreaHub.Application.Notificaciones;
using UrreaHub.Domain.Auditoria;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class LoggingNotificationSender : INotificationSender
{
    private readonly UrreaHubDbContext _context;
    private readonly ILogger<LoggingNotificationSender> _logger;

    public LoggingNotificationSender(UrreaHubDbContext context, ILogger<LoggingNotificationSender> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SendAsync(NotificationRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Notification [{Evento}] to {Dest}: {Asunto}", request.Evento, request.DestinatarioEmail, request.Asunto);

        _context.NotificacionesEnviadas.Add(new NotificacionEnviada
        {
            Id = Guid.NewGuid(),
            Canal = "Log",
            Destinatario = request.DestinatarioEmail,
            Asunto = request.Asunto,
            Contenido = request.Contenido,
            FechaEnvio = DateTime.UtcNow,
            Exitosa = true,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        });
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class GraphNotificationSender : INotificationSender
{
    private readonly UrreaHubDbContext _context;
    private readonly AzureAdSettings _settings;
    private readonly ILogger<GraphNotificationSender> _logger;
    private GraphServiceClient? _graph;

    public GraphNotificationSender(
        UrreaHubDbContext context,
        IOptions<AzureAdSettings> settings,
        ILogger<GraphNotificationSender> logger)
    {
        _context = context;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendAsync(NotificationRequest request, CancellationToken cancellationToken = default)
    {
        var exitosa = false;
        string? error = null;

        try
        {
            if (!string.IsNullOrEmpty(_settings.TeamsWebhookUrl))
                await SendTeamsWebhookAsync(request, cancellationToken);

            if (HasGraphConfig())
                await SendOutlookEmailAsync(request, cancellationToken);

            exitosa = true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            _logger.LogError(ex, "Failed to send notification");
        }

        _context.NotificacionesEnviadas.Add(new NotificacionEnviada
        {
            Id = Guid.NewGuid(),
            Canal = HasGraphConfig() ? "Graph" : "TeamsWebhook",
            Destinatario = request.DestinatarioEmail,
            Asunto = request.Asunto,
            Contenido = request.Contenido,
            FechaEnvio = DateTime.UtcNow,
            Exitosa = exitosa,
            Error = error,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        });
        await _context.SaveChangesAsync(cancellationToken);
    }

    private bool HasGraphConfig() =>
        !string.IsNullOrEmpty(_settings.TenantId) &&
        !string.IsNullOrEmpty(_settings.ClientId) &&
        !string.IsNullOrEmpty(_settings.ClientSecret);

    private GraphServiceClient GetGraphClient()
    {
        _graph ??= new GraphServiceClient(new ClientSecretCredential(
            _settings.TenantId, _settings.ClientId, _settings.ClientSecret));
        return _graph;
    }

    private async Task SendOutlookEmailAsync(NotificationRequest request, CancellationToken cancellationToken)
    {
        var message = new Message
        {
            Subject = request.Asunto,
            Body = new ItemBody { ContentType = BodyType.Text, Content = request.Contenido },
            ToRecipients = new List<Recipient>
            {
                new() { EmailAddress = new EmailAddress { Address = request.DestinatarioEmail } }
            }
        };

        await GetGraphClient().Users[_settings.FromEmail].SendMail.PostAsync(
            new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
            {
                Message = message,
                SaveToSentItems = false
            }, cancellationToken: cancellationToken);
    }

    private async Task SendTeamsWebhookAsync(NotificationRequest request, CancellationToken cancellationToken)
    {
        using var http = new HttpClient();
        var payload = $$"""
        {
          "@type": "MessageCard",
          "@context": "http://schema.org/extensions",
          "summary": "{{request.Asunto}}",
          "themeColor": "0076D7",
          "title": "{{request.Asunto}}",
          "text": "{{request.Contenido.Replace("\"", "\\\"")}}"
        }
        """;
        var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");
        await http.PostAsync(_settings.TeamsWebhookUrl, content, cancellationToken);
    }
}
