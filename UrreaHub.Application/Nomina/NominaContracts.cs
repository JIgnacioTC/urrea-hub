namespace UrreaHub.Application.Nomina;

public record ColaboradorNominaRecord(
    string NumeroEmpleado,
    string Nombre,
    string ApellidoPaterno,
    string? ApellidoMaterno,
    string Email,
    string? Rfc,
    DateTime FechaIngreso,
    DateTime? FechaBaja);

public class NominaSyncSettings
{
    public const string SectionName = "NominaSync";
    public bool Enabled { get; set; } = true;
    public int IntervalMinutes { get; set; } = 60;
    public string Adapter { get; set; } = "Stub";
    public string? CsvPath { get; set; }
}

public interface INominaSyncAdapter
{
    Task<IReadOnlyList<ColaboradorNominaRecord>> FetchColaboradoresAsync(CancellationToken cancellationToken = default);
}

public interface INominaSyncService
{
    Task<int> SyncAsync(CancellationToken cancellationToken = default);
}
