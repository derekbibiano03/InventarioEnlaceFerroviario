using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.DTOs.Materiales
{
    public class MaterialesDto
    {
        public int IdMaterial { get; set; }

        public string DescripcionCorta { get; set; }

        public string DescripcionLarga { get; set; } 
        
        public string Marca { get; set; }

        public string NoParte { get; set; }

        public string Grupo { get; set; } 

        public string Clasificacion { get; set; } 
        public string Subclasificacion { get; set; }
        public string UnidadMedida { get; set; }
        public string Detalles { get; set; }

        public virtual CatalogoClasificacione? IdClasificacionNavigation { get; set; }
        public virtual CatalogoSubclasificacione? IdSubclasificacionNavigation { get; set; }
        public virtual CatalogoGrupo? IdGrupoNavigation { get; set; }
        public virtual CatalogoMarca? MarcaNavigation { get; set; }

    }
}
