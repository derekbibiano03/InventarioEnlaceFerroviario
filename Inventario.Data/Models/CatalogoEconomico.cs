using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario.Data.Models;

public partial class CatalogoEconomico
{
    [NotMapped]
    public bool IsSelected { get; set; }
    public string IdEconomico { get; set; } = null!;

    public int? Consecutivo { get; set; }

    public string? IdTipoEquipo { get; set; }

    public string? IdGrupo { get; set; }

    public int? IdCombustible { get; set; }

    public int? IdPropietario { get; set; }

    public int? IdAdministrador { get; set; }

    public int? IdEstatus { get; set; }

    public int? IdUbicacion { get; set; }

    public string? Descripcion { get; set; }

    public string? Modelo { get; set; }

    public string? Serie { get; set; }

    public int? PeriodoFabricacion { get; set; }

    public string? Motor { get; set; }

    public string? ModeloMotor { get; set; }

    public string? SerieMotor { get; set; }

    public string? FamiliaMotor { get; set; }

    public decimal Horometro { get; set; }

    public string? Dimensiones { get; set; }

    public string? Thk { get; set; }

    public string? Placas { get; set; }

    public string? GradoPropiedad { get; set; }

    public string? ObservacionesAsignaciones { get; set; }

    public bool? EstatusSeguro { get; set; }

    public int? IdMarca { get; set; }

    public int? MarcaMotor { get; set; }

    public decimal? ValorAdquisicion { get; set; }

    public string? TipoSeguro { get; set; }

    public int? IdOperador { get; set; }

    public int? IdResponsable { get; set; }

    public bool? Verificado { get; set; }

    public string? IdSamsara { get; set; }

    public int? Kmhrmantenimiento { get; set; }

    public virtual ICollection<CatalogoMovimientosEconomico> CatalogoMovimientosEconomicos { get; set; } = new List<CatalogoMovimientosEconomico>();

    public virtual ICollection<EconomicosArchivo> EconomicosArchivos { get; set; } = new List<EconomicosArchivo>();

    public virtual ICollection<HistorialReporte> HistorialReportes { get; set; } = new List<HistorialReporte>();

    public virtual ICollection<HistorialResguardo> HistorialResguardos { get; set; } = new List<HistorialResguardo>();

    public virtual ICollection<HistorialServicio> HistorialServicios { get; set; } = new List<HistorialServicio>();

    public virtual CatalogoPya? IdAdministradorNavigation { get; set; }

    public virtual CatalogoTiposCombustible? IdCombustibleNavigation { get; set; }

    public virtual CatalogoEstatus? IdEstatusNavigation { get; set; }

    public virtual CatalogoGrupo? IdGrupoNavigation { get; set; }

    public virtual CatalogoMarca? IdMarcaNavigation { get; set; }

    public virtual Empleado? IdOperadorNavigation { get; set; }

    public virtual CatalogoPya? IdPropietarioNavigation { get; set; }

    public virtual Empleado? IdResponsableNavigation { get; set; }

    public virtual CatalogoTiposEquipo? IdTipoEquipoNavigation { get; set; }

    public virtual CatalogoUbicacionesProyecto? IdUbicacionNavigation { get; set; }

    public virtual CatalogoMarca? MarcaMotorNavigation { get; set; }

    public virtual ICollection<Requisicione> Requisiciones { get; set; } = new List<Requisicione>();
}
