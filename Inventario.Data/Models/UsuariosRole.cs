using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class UsuariosRole
{
    public int IdRol { get; set; }

    public string? DescripcionRol { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
