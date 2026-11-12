using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class HistorialReporte
{
    public string IdReporte { get; set; } = null!;

    public string IdEconomico { get; set; } = null!;

    public int? IdUbicacion { get; set; }

    public int? IdTipoReporte { get; set; }

    public int? Horometro { get; set; }

    public int? IdUsuario { get; set; }

    public int? IdNuevoEstatus { get; set; }

    public string? UbicacionDetalle { get; set; }

    public string? DescripcionReporte { get; set; }

    public string? PrioridadAtencion { get; set; }

    public virtual CatalogoEconomico IdEconomicoNavigation { get; set; } = null!;

    public virtual CatalogoEstatus? IdNuevoEstatusNavigation { get; set; }

    public virtual TiposReporte? IdTipoReporteNavigation { get; set; }

    public virtual CatalogoUbicacionesProyecto? IdUbicacionNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
