using Inventario.Data;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.Services.UbicacionProyecto
{
    public class UbicacionProyeectoService
    {
        private readonly InventarioContext _context;

        public UbicacionProyeectoService(InventarioContext context)
        {
            _context = context;
        }

        public List<CatalogoUbicacionesProyecto> ObtenerUbicaciones()
        {
            var resultado = _context.CatalogoUbicacionesProyectos.OrderBy(x => x.Ubicacion).ToList();
            return resultado;
        }
    }
}
