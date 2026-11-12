using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class Empleado
{
    public int NoEmpleado { get; set; }

    public string? NombreEmpleado { get; set; }

    public int? IdRolEmpleado { get; set; }

    public DateOnly? Ds3 { get; set; }

    public int? IdUbicacion { get; set; }

    public virtual ICollection<CatalogoEconomico> CatalogoEconomicoIdOperadorNavigations { get; set; } = new List<CatalogoEconomico>();

    public virtual ICollection<CatalogoEconomico> CatalogoEconomicoIdResponsableNavigations { get; set; } = new List<CatalogoEconomico>();

    public virtual ICollection<HistorialResguardo> HistorialResguardos { get; set; } = new List<HistorialResguardo>();

    public virtual RolEmpleado? IdRolEmpleadoNavigation { get; set; }

    public virtual CatalogoUbicacionesProyecto? IdUbicacionNavigation { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
