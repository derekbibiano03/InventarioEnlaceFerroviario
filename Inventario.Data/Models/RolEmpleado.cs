using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class RolEmpleado
{
    public int IdRolEmpleado { get; set; }

    public string? DescripcionRol { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
