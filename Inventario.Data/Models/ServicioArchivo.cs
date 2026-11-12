using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class ServicioArchivo
{
    public int IdServicioArchivo { get; set; }

    public int? IdArchivo { get; set; }

    public int? IdServicio { get; set; }

    public virtual CatalogoArchivo? IdArchivoNavigation { get; set; }

    public virtual HistorialServicio? IdServicioNavigation { get; set; }
}
