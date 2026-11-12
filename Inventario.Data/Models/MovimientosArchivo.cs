using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class MovimientosArchivo
{
    public int Id { get; set; }

    public int IdMovimiento { get; set; }

    public int IdArchivo { get; set; }

    public virtual CatalogoArchivo IdArchivoNavigation { get; set; } = null!;

    public virtual CatalogoMovimientosEconomico IdMovimientoNavigation { get; set; } = null!;
}
