using Inventario.Core.DTOs;
using Inventario.Core.DTOs.Materiales;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.Services.Catalogos
{
    public class CatalogoMateriales
    {
        private readonly InventarioContext _context;

        public CatalogoMateriales(InventarioContext context) 
        { 
            _context = context;
        }


        public List<MaterialesDto> ObtenerMaterialInfo ()
        {
            var resultado = _context.CatalogoMateriales
                .Include(e => e.MarcaNavigation)
                .Include(e => e.IdClasificacionNavigation)
                .Include(e => e.IdSubclasificacionNavigation)
                .Include(e => e.IdGrupoNavigation)
                .Select(e => new MaterialesDto
                {
                    IdMaterial = e.IdMaterial,
                    DescripcionCorta = e.DescripcionCorta,
                    DescripcionLarga = e.DescripcionLarga,
                    Marca = e.MarcaNavigation != null ? e.MarcaNavigation.NombreMarca : "SIN MARCA",
                    NoParte = e.NoParte,
                    UnidadMedida = e.UnidadMedida,
                    Detalles = e.Detalles,
                    Grupo = e.IdGrupoNavigation != null ? e.IdGrupoNavigation.DescripcionGrupo : "SIN ASIGNAR A UNA MAQUINA",
                    Clasificacion = e.IdClasificacionNavigation != null ? e.IdClasificacionNavigation.Descripcion : "SIN CLASIFICAR",
                    Subclasificacion = e.IdSubclasificacionNavigation != null ? e.IdSubclasificacionNavigation.Descripcion : "SIN SUBCLASIFICAR"
                })
                .ToList();
            return resultado;
        }




    }
}
