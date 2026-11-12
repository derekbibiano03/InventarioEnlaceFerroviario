using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoProveedore
{
    public int IdProveedor { get; set; }

    public string? RazonSocial { get; set; }

    public string? NumeroTelefonico { get; set; }

    public string? CorreoElectronico { get; set; }

    public string? Calle { get; set; }

    public string? Numero { get; set; }

    public string? Colonia { get; set; }

    public string? CodigoP { get; set; }

    public string? Ciudad { get; set; }

    public string? Estado { get; set; }

    public string? Pais { get; set; }

    public string? Rfc { get; set; }

    public string? Contacto { get; set; }

    public string? Constancia { get; set; }

    public virtual ICollection<OrdenesCompra> OrdenesCompras { get; set; } = new List<OrdenesCompra>();
}
