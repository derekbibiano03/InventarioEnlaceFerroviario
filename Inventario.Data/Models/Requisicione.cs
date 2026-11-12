using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class Requisicione
{
    public string IdRequisicion { get; set; } = null!;

    public int? IdUbicacion { get; set; }

    public int? Consecutivo { get; set; }

    public DateOnly? FechaRequisicion { get; set; }

    public string? TipoRequisicion { get; set; }

    public string? Empresa { get; set; }

    public string? Estatus { get; set; }

    public string? ArchivoReq { get; set; }

    public int? IdSolicitante { get; set; }

    public int? IdAutorizante { get; set; }

    public int? IdAtencion { get; set; }

    public int? IdAutorizante2 { get; set; }

    public string? AreaRequisicion { get; set; }

    public string? IdEconomico { get; set; }

    public string? GradoPrioridad { get; set; }

    public virtual Usuario? IdAtencionNavigation { get; set; }

    public virtual Usuario? IdAutorizante2Navigation { get; set; }

    public virtual Usuario? IdAutorizanteNavigation { get; set; }

    public virtual CatalogoEconomico? IdEconomicoNavigation { get; set; }

    public virtual Usuario? IdSolicitanteNavigation { get; set; }

    public virtual CatalogoUbicacionesProyecto? IdUbicacionNavigation { get; set; }

    public virtual ICollection<OrdenesCompra> OrdenesCompras { get; set; } = new List<OrdenesCompra>();
}
