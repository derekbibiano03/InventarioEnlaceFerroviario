using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string? NombreUsuario { get; set; }

    public string? Password { get; set; }

    public int? IdRol { get; set; }

    public string? NombreCompleto { get; set; }

    public int? NoEmpleado { get; set; }

    public string? Correoe { get; set; }

    public string? Area { get; set; }

    public string? FirmaPath { get; set; }

    public virtual ICollection<CatalogoMovimientosEconomico> CatalogoMovimientosEconomicos { get; set; } = new List<CatalogoMovimientosEconomico>();

    public virtual ICollection<HistorialLog> HistorialLogs { get; set; } = new List<HistorialLog>();

    public virtual ICollection<HistorialReporte> HistorialReportes { get; set; } = new List<HistorialReporte>();

    public virtual ICollection<HistorialServicio> HistorialServicios { get; set; } = new List<HistorialServicio>();

    public virtual UsuariosRole? IdRolNavigation { get; set; }

    public virtual Empleado? NoEmpleadoNavigation { get; set; }

    public virtual ICollection<Requisicione> RequisicioneIdAtencionNavigations { get; set; } = new List<Requisicione>();

    public virtual ICollection<Requisicione> RequisicioneIdAutorizante2Navigations { get; set; } = new List<Requisicione>();

    public virtual ICollection<Requisicione> RequisicioneIdAutorizanteNavigations { get; set; } = new List<Requisicione>();

    public virtual ICollection<Requisicione> RequisicioneIdSolicitanteNavigations { get; set; } = new List<Requisicione>();
}
