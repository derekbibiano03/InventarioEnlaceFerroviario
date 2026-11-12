using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class TiposReporte
{
    public int IdTipoReporte { get; set; }

    public string? DescripcionTipoReporte { get; set; }

    public string? Siglas { get; set; }

    public virtual ICollection<HistorialReporte> HistorialReportes { get; set; } = new List<HistorialReporte>();
}
