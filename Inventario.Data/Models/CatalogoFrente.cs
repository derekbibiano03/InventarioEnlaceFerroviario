using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoFrente
{
    public int IdFrente { get; set; }

    public int? IdTramo { get; set; }

    public string? NombreFrente { get; set; }

    public virtual CatalogoTramo? IdTramoNavigation { get; set; }
}
