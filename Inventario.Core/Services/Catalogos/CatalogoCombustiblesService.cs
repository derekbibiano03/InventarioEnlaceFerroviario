using Inventario.Data;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.Services.Catalogos
{
    public class CatalogoCombustiblesService
    {

        private readonly InventarioContext _context;

        public CatalogoCombustiblesService(InventarioContext context)
        {
            _context = context;
        }

        public List<CatalogoTiposCombustible> ObtenerCombustibles()
        {

            var resultado = _context.CatalogoTiposCombustibles.ToList();
            return resultado;

        }
    }
}
