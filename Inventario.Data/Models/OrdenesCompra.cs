using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class OrdenesCompra
{
    public string IdOrdenCompra { get; set; } = null!;

    public string? IdRequisicion { get; set; }

    public int? IdUbicacion { get; set; }

    public int? IdProveedor { get; set; }

    public string? Empresa { get; set; }

    public string? NoCotizacion { get; set; }

    public string? LugarEntrega { get; set; }

    public string? ArchivoOrdenCompra { get; set; }

    public virtual CatalogoProveedore? IdProveedorNavigation { get; set; }

    public virtual Requisicione? IdRequisicionNavigation { get; set; }

    public virtual CatalogoUbicacionesProyecto? IdUbicacionNavigation { get; set; }
}
