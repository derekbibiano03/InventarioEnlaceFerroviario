using Inventario.Data;
using System;
using Inventario.Data.Models;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.Services.Catalogos
{
    public class CatalogoMarcasService
    {

        private readonly InventarioContext _context;

        public CatalogoMarcasService(InventarioContext context)
        {
            _context = context;
        }

        public List<CatalogoMarca> ObtenerMarcas()
        {
        
            var resultado = _context.CatalogoMarcas.OrderBy(x => x.NombreMarca).ToList();
            return resultado;

        }

        public bool NuevaMarca(string nombreMarca)
        {
            var nuevaMarca = new CatalogoMarca
            {
                NombreMarca = nombreMarca
            };

            _context.CatalogoMarcas.Add(nuevaMarca);
            _context.SaveChanges();
            return true;
        }

    }
}
