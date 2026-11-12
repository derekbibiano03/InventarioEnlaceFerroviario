using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class EconomicosArchivo
{
    public int IdEconomicoArchivo { get; set; }

    public int? IdArchivo { get; set; }

    public string? IdEconomico { get; set; }

    public virtual CatalogoArchivo? IdArchivoNavigation { get; set; }

    public virtual CatalogoEconomico? IdEconomicoNavigation { get; set; }
}
