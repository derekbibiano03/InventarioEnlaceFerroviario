using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoSubclasificacione
{
    public string IdSubclasificacion { get; set; } = null!;

    public string? IdClasificacion { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<CatalogoMateriale> CatalogoMateriales { get; set; } = new List<CatalogoMateriale>();

    public virtual CatalogoClasificacione? IdClasificacionNavigation { get; set; }
}
