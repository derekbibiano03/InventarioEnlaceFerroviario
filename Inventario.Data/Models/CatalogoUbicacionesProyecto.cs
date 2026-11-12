using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoUbicacionesProyecto
{
    public int IdUbicacion { get; set; }

    public string NombreProyecto { get; set; } = null!;

    public string Ubicacion { get; set; } = null!;

    public string? Siglas { get; set; }

    public virtual ICollection<CatalogoEconomico> CatalogoEconomicos { get; set; } = new List<CatalogoEconomico>();

    public virtual ICollection<CatalogoMovimientosEconomico> CatalogoMovimientosEconomicoIdUbicacionLlegadaNavigations { get; set; } = new List<CatalogoMovimientosEconomico>();

    public virtual ICollection<CatalogoMovimientosEconomico> CatalogoMovimientosEconomicoIdUbicacionSalidaNavigations { get; set; } = new List<CatalogoMovimientosEconomico>();

    public virtual ICollection<CatalogoTramo> CatalogoTramos { get; set; } = new List<CatalogoTramo>();

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();

    public virtual ICollection<HistorialReporte> HistorialReportes { get; set; } = new List<HistorialReporte>();

    public virtual ICollection<HistorialServicio> HistorialServicios { get; set; } = new List<HistorialServicio>();

    public virtual ICollection<OrdenesCompra> OrdenesCompras { get; set; } = new List<OrdenesCompra>();

    public virtual ICollection<Requisicione> Requisiciones { get; set; } = new List<Requisicione>();
}
