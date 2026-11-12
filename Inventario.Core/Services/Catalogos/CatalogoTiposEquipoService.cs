using Inventario.Data;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.Services.Catalogos
{
    public class CatalogoTiposEquipoService
    {

        private readonly InventarioContext _context;

        public CatalogoTiposEquipoService(InventarioContext context)
        {
            _context = context;
        }

        public List<CatalogoTiposEquipo> ObtenerTiposEq()
        {
            var resultado = _context.CatalogoTiposEquipos.ToList();
            return resultado;
        }

    }
}
