using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoTramo
{
    public int IdTramo { get; set; }

    public int? IdUbicacion { get; set; }

    public string? NombreTramo { get; set; }

    public virtual ICollection<CatalogoFrente> CatalogoFrentes { get; set; } = new List<CatalogoFrente>();

    public virtual CatalogoUbicacionesProyecto? IdUbicacionNavigation { get; set; }
}
