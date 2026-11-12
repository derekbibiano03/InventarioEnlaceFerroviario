using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoEstatus
{
    public int IdEstatus { get; set; }

    public string DescripcionEstatus { get; set; } = null!;

    public virtual ICollection<CatalogoEconomico> CatalogoEconomicos { get; set; } = new List<CatalogoEconomico>();

    public virtual ICollection<HistorialReporte> HistorialReportes { get; set; } = new List<HistorialReporte>();
}
