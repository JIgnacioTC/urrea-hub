using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrreaHub.Domain.Catalogos;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Api.Controllers;

[Authorize(Policy = "RhAdmin")]
[Route("api/catalogs/admin/[controller]")]
public class CatalogosEstadosController : CrudControllerBase<CatalogoEstado>
{
    public CatalogosEstadosController(UrreaHubDbContext context) : base(context) { }
}

[Authorize(Policy = "RhAdmin")]
[Route("api/catalogs/admin/[controller]")]
public class CatalogosMunicipiosController : CrudControllerBase<CatalogoMunicipio>
{
    public CatalogosMunicipiosController(UrreaHubDbContext context) : base(context) { }
}

[Authorize(Policy = "RhAdmin")]
[Route("api/catalogs/admin/[controller]")]
public class CatalogosEstadosCivilesController : CrudControllerBase<CatalogoEstadoCivil>
{
    public CatalogosEstadosCivilesController(UrreaHubDbContext context) : base(context) { }
}

[Authorize(Policy = "RhAdmin")]
[Route("api/catalogs/admin/[controller]")]
public class RazonesTerminoController : CrudControllerBase<RazonTermino>
{
    public RazonesTerminoController(UrreaHubDbContext context) : base(context) { }
}

[Authorize(Policy = "RhAdmin")]
[Route("api/catalogs/admin/[controller]")]
public class RegistrosPatronalesController : CrudControllerBase<RegistroPatronal>
{
    public RegistrosPatronalesController(UrreaHubDbContext context) : base(context) { }
}

[Authorize(Policy = "RhAdmin")]
[Route("api/catalogs/admin/[controller]")]
public class CatalogosJerarquiasController : CrudControllerBase<CatalogoJerarquia>
{
    public CatalogosJerarquiasController(UrreaHubDbContext context) : base(context) { }
}
