using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoGrupo
{
    public string IdGrupo { get; set; } = null!;

    public string DescripcionGrupo { get; set; } = null!;

    public virtual ICollection<CatalogoEconomico> CatalogoEconomicos { get; set; } = new List<CatalogoEconomico>();

    public virtual ICollection<CatalogoMateriale> CatalogoMateriales { get; set; } = new List<CatalogoMateriale>();

    public virtual ICollection<HistorialServicio> HistorialServicios { get; set; } = new List<HistorialServicio>();
}
