using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class HistorialResguardo
{
    public int IdResguardo { get; set; }

    public string? IdEconomico { get; set; }

    public int? IdOperador { get; set; }

    public DateOnly? FechaResguardo { get; set; }

    public string? Observaciones { get; set; }

    public virtual CatalogoEconomico? IdEconomicoNavigation { get; set; }

    public virtual Empleado? IdOperadorNavigation { get; set; }

    public virtual ICollection<ResguardosArchivo> ResguardosArchivos { get; set; } = new List<ResguardosArchivo>();
}
