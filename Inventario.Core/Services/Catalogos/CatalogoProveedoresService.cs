using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.Services.Catalogos
{
    public class CatalogoProveedoresService
    {
        private readonly InventarioContext _context;

        public CatalogoProveedoresService(InventarioContext context)
        {
            _context = context;
        }

        public List<CatalogoProveedore> ObtenerProveedores() 
        {
            var resultado = _context.CatalogoProveedores.ToList();
            return resultado;
        }
    }
}
