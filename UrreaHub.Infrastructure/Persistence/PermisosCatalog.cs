using Microsoft.EntityFrameworkCore;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.Vacaciones;

namespace UrreaHub.Infrastructure.Persistence;

public static class PermisosCatalog
{
    public record TipoPermisoSeed(
        string Codigo,
        string Nombre,
        CategoriaPermiso Categoria,
        bool EsParcial,
        bool PermiteMultiDia,
        bool DescuentaSaldo,
        bool Remunerado,
        bool RequiereComprobante,
        decimal? DiasMaximosAnuales,
        decimal? DiasMaximosEvento,
        string BaseLegalLft,
        string Descripcion,
        string Icono,
        string Color,
        int Orden);

    public static readonly TipoPermisoSeed[] Tipos = new TipoPermisoSeed[]
    {
        new("VAC", "Vacaciones", CategoriaPermiso.Vacacion, false, true, true, true, false, null, null,
            "Art. 76 LFT",
            "Periodo anual de descanso con goce de salario. Mínimo 12 días después del primer año (reforma 2023).",
            "🏖", "#10b981", 1),
        new("HOME_OFFICE", "Home Office", CategoriaPermiso.PermisoDiaCompleto, false, true, false, true, false, null, null,
            "Art. 330-D LFT",
            "Modalidad de teletrabajo en domicilio del trabajador, sujeta a acuerdo con el patrón.",
            "🏠", "#6366f1", 2),
        new("SALIDA_TEMPRANO", "Salida Temprano", CategoriaPermiso.PermisoParcial, true, false, false, true, false, null, null,
            "Art. 25 y 29 LFT",
            "Autorización para retirarse antes del horario laboral. No descuenta vacaciones; debe registrarse el horario.",
            "🚪", "#f59e0b", 3),
        new("PERMISO_LABORAL", "Permiso Laboral", CategoriaPermiso.PermisoDiaCompleto, false, true, false, false, true, 3, null,
            "Arts. 29, 32 y 42 LFT",
            "Ausencia por causa justificada sin goce de sueldo, dentro del límite de faltas permitidas por la ley.",
            "📋", "#64748b", 4),
        new("ENTRADA_TARDE", "Entrada Tarde", CategoriaPermiso.PermisoParcial, true, false, false, true, false, null, null,
            "Art. 25 y 29 LFT",
            "Autorización para ingresar después del horario establecido. Registra la hora estimada de llegada.",
            "⏰", "#eab308", 5),
        new("DIA_VALOR", "Día Valor", CategoriaPermiso.PermisoDiaCompleto, false, false, false, true, false, null, null,
            "Art. 75 LFT",
            "Descanso por día de descanso semanal laborado. Debe otorgarse dentro de los 3 días siguientes o pagarse doble.",
            "⭐", "#0ea5e9", 6),
        new("MATRIMONIO", "Matrimonio", CategoriaPermiso.LicenciaLegal, false, false, false, true, true, null, 5,
            "Art. 5 y 132 LFT; práctica laboral",
            "Licencia por matrimonio civil. URREA otorga hasta 5 días con goce conforme a política interna y buenas prácticas.",
            "💍", "#ec4899", 7),
        new("PATERNIDAD", "Paternidad", CategoriaPermiso.LicenciaLegal, false, false, false, true, true, null, 5,
            "Art. 132 fracc. XXII bis LFT",
            "Licencia de paternidad por nacimiento o adopción: mínimo 5 días laborables con goce de salario (reforma 2023).",
            "👶", "#14b8a6", 8),
    };

    public static async Task UpsertTiposAsync(UrreaHubDbContext context)
    {
        var existing = await context.TiposAusencia.ToListAsync();
        var now = DateTime.UtcNow;

        foreach (var seed in Tipos)
        {
            var tipo = existing.FirstOrDefault(t => t.Codigo == seed.Codigo);
            if (tipo is null)
            {
                context.TiposAusencia.Add(new TipoAusencia
                {
                    Id = Guid.NewGuid(),
                    Codigo = seed.Codigo,
                    Nombre = seed.Nombre,
                    Categoria = seed.Categoria,
                    EsParcial = seed.EsParcial,
                    PermiteMultiDia = seed.PermiteMultiDia,
                    DescuentaSaldo = seed.DescuentaSaldo,
                    Remunerado = seed.Remunerado,
                    RequiereComprobante = seed.RequiereComprobante,
                    DiasMaximosAnuales = seed.DiasMaximosAnuales,
                    DiasMaximosEvento = seed.DiasMaximosEvento,
                    BaseLegalLft = seed.BaseLegalLft,
                    Descripcion = seed.Descripcion,
                    Icono = seed.Icono,
                    Color = seed.Color,
                    RequiereAprobacion = true,
                    Orden = seed.Orden,
                    CreatedAt = now,
                    IsActive = true,
                });
                continue;
            }

            tipo.Nombre = seed.Nombre;
            tipo.Categoria = seed.Categoria;
            tipo.EsParcial = seed.EsParcial;
            tipo.PermiteMultiDia = seed.PermiteMultiDia;
            tipo.DescuentaSaldo = seed.DescuentaSaldo;
            tipo.Remunerado = seed.Remunerado;
            tipo.RequiereComprobante = seed.RequiereComprobante;
            tipo.DiasMaximosAnuales = seed.DiasMaximosAnuales;
            tipo.DiasMaximosEvento = seed.DiasMaximosEvento;
            tipo.BaseLegalLft = seed.BaseLegalLft;
            tipo.Descripcion = seed.Descripcion;
            tipo.Icono = seed.Icono;
            tipo.Color = seed.Color;
            tipo.Orden = seed.Orden;
            tipo.UpdatedAt = now;
            tipo.IsActive = true;
        }

        var legacyPerm = existing.FirstOrDefault(t => t.Codigo == "PERM");
        if (legacyPerm is not null)
            legacyPerm.IsActive = false;

        await context.SaveChangesAsync();
    }
}
