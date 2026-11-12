using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class ResguardosArchivo
{
    public int IdResguardoArchivo { get; set; }

    public int? IdResguardo { get; set; }

    public int? IdArchivo { get; set; }

    public virtual CatalogoArchivo? IdArchivoNavigation { get; set; }

    public virtual HistorialResguardo? IdResguardoNavigation { get; set; }
}
