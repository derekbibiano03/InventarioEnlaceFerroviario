using Inventario.Data;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.Services.Catalogos
{
    public class CatalogoGruposService
    {

        private readonly InventarioContext _context;

        public CatalogoGruposService(InventarioContext context)
        {

            _context = context;

        }

        public List<CatalogoGrupo> ObtenerGrupos()
        {

            var resultado = _context.CatalogoGrupos.OrderBy(x => x.DescripcionGrupo).ToList();
            return resultado;

        }

        public bool NuevoGrupo(string descripcionCorta, string descripcionCompleta) 
        {
            var nuevoGrupo = new CatalogoGrupo 
            { 
                IdGrupo = descripcionCorta.ToLower(),
                DescripcionGrupo = descripcionCompleta
            };

            _context.CatalogoGrupos.Add(nuevoGrupo);
            _context.SaveChanges();
            return true;
        }
    }
}
