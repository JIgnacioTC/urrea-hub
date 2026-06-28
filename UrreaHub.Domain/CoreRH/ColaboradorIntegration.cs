namespace UrreaHub.Domain.CoreRH;

public static class EmployeeSyncStatus
{
    public const string Pending = "Pending";
    public const string Synced = "Synced";
    public const string Error = "Error";
    public const string Conflict = "Conflict";
    public const string ManualOverride = "ManualOverride";
}

public static class EmployeeExternalSource
{
    public const string Manual = "Manual";
    public const string SapCdm = "SAP/CDM";
    public const string Nomina = "Nomina";
}
