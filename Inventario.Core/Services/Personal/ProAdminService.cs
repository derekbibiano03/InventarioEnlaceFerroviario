using Inventario.Data;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.Services.Personal
{
    public class ProAdminService
    {
        private readonly InventarioContext _context;

        public ProAdminService(InventarioContext context)
        {
            _context = context;
        }

        public List<CatalogoPya> ObtenerPYA()
        {
            var resultado = _context.CatalogoPyas
                .Select(e => new CatalogoPya
                {
                    IdPya = e.IdPya,
                    Nombre = e.Nombre

                })
                .ToList();
            return resultado;
        }
    }
}
