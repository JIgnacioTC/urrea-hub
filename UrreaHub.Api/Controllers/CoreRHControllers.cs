using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrreaHub.Domain.CoreRH;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Api.Controllers;

[Authorize(Policy = "RhAdmin")]
[Route("api/core-rh/[controller]")]
public class ColaboradoresController : CrudControllerBase<Colaborador>
{
    public ColaboradoresController(UrreaHubDbContext context) : base(context) { }
}

[Authorize(Policy = "RhAdmin")]
[Route("api/core-rh/[controller]")]
public class PuestosController : CrudControllerBase<Puesto>
{
    public PuestosController(UrreaHubDbContext context) : base(context) { }
}

[Authorize(Policy = "RhAdmin")]
[Route("api/core-rh/[controller]")]
public class AreasController : CrudControllerBase<Area>
{
    public AreasController(UrreaHubDbContext context) : base(context) { }
}

[Authorize(Policy = "RhAdmin")]
[Route("api/core-rh/[controller]")]
public class DepartamentosController : CrudControllerBase<Departamento>
{
    public DepartamentosController(UrreaHubDbContext context) : base(context) { }
}

[Authorize(Policy = "RhAdmin")]
[Route("api/core-rh/[controller]")]
public class SedesController : CrudControllerBase<Sede>
{
    public SedesController(UrreaHubDbContext context) : base(context) { }
}

[Authorize(Policy = "RhAdmin")]
[Route("api/core-rh/[controller]")]
public class CentrosCostoController : CrudControllerBase<CentroCosto>
{
    public CentrosCostoController(UrreaHubDbContext context) : base(context) { }
}

[Authorize(Policy = "RhAdmin")]
[Route("api/core-rh/[controller]")]
public class RelacionesLaboralesController : CrudControllerBase<RelacionLaboral>
{
    public RelacionesLaboralesController(UrreaHubDbContext context) : base(context) { }
}
