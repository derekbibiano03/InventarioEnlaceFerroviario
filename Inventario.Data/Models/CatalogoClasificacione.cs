using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoClasificacione
{
    public string IdClasificacion { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<CatalogoMateriale> CatalogoMateriales { get; set; } = new List<CatalogoMateriale>();

    public virtual ICollection<CatalogoSubclasificacione> CatalogoSubclasificaciones { get; set; } = new List<CatalogoSubclasificacione>();
}
