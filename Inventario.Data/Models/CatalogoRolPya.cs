using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoRolPya
{
    public int IdRol { get; set; }

    public string? DescripcionRol { get; set; }

    public virtual ICollection<CatalogoPya> CatalogoPyas { get; set; } = new List<CatalogoPya>();
}
