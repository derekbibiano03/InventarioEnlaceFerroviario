using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.DTOs
{
    public class EconomicoMinimoDto
    {
        public string? IdEconomico { get; set; }
        public string? IdTipoEquipo { get; set; }
        public int? IdUbicacion { get; set; }
        public string? Descripcion { get; set; }
        public int? IdMarca { get; set; }
        public string? Modelo { get; set; }
        public string? Serie { get; set; }
        public int? PeriodoFabricacion { get; set; }
        public string? NombreMarca { get; set; }
        public float? Horometro { get; set; }

        public string? TipoSeguro { get; set; }

        public virtual CatalogoTiposEquipo? IdTipoEquipoNavigation { get; set; }
        public virtual CatalogoUbicacionesProyecto? IdUbicacionNavigation { get; set; }
        public virtual CatalogoMarca? IdMarcaNavigation { get; set; }
    }
}
