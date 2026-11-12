using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoMovimientosEconomico
{
    public int IdMovimiento { get; set; }

    public string? IdEconomico { get; set; }

    public int? IdUbicacionLlegada { get; set; }

    public int? IdUbicacionSalida { get; set; }

    public DateTime? FechaMovimiento { get; set; }

    public int? IdUsuario { get; set; }

    public DateTime? FechaLlegada { get; set; }

    public string? Anotaciones { get; set; }

    public string? AnotacionesLlegada { get; set; }

    public virtual CatalogoEconomico? IdEconomicoNavigation { get; set; }

    public virtual CatalogoUbicacionesProyecto? IdUbicacionLlegadaNavigation { get; set; }

    public virtual CatalogoUbicacionesProyecto? IdUbicacionSalidaNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }

    public virtual ICollection<MovimientosArchivo> MovimientosArchivos { get; set; } = new List<MovimientosArchivo>();
}
