using Inventario.Data;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.Services.Catalogos
{
    public class CatalogoEstatusService
    {

        private readonly InventarioContext _context;

        public CatalogoEstatusService(InventarioContext context)
        {
            _context = context;
        }

        public List<CatalogoEstatus> ObtenerEstatus()
        {
            var resultado = _context.CatalogoEstatuses.OrderBy(x => x.DescripcionEstatus).ToList();
            return resultado;
        }

    }
}
