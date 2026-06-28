using Microsoft.EntityFrameworkCore;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Persistence;

public static class TiPlatformSeed
{
    public static async Task SeedAsync(UrreaHubDbContext context)
    {
        var patricia = await context.CuentasAcceso
            .Include(c => c.Colaborador)
            .FirstOrDefaultAsync(c => c.Colaborador.NumeroEmpleado == "1005");

        if (patricia is not null && !patricia.EsTiAdmin)
        {
            patricia.EsTiAdmin = true;
            await context.SaveChangesAsync();
        }
    }
}
