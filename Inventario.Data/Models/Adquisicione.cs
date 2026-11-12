using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class Adquisicione
{
    public string? IdAdquisicion { get; set; }

    public int? IdMaterial { get; set; }

    public decimal? Cantidad { get; set; }

    public decimal? PrecioUnitario { get; set; }

    public string? NoParte { get; set; }

    public virtual CatalogoMateriale? IdMaterialNavigation { get; set; }
}
