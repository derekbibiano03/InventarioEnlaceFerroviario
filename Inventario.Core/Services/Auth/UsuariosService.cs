using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.Services.Auth
{
    public class UsuariosService
    {
        private readonly InventarioContext _context;

        public UsuariosService(InventarioContext context)
        {
            _context = context;
        }

        public List<Usuario> ObtenerUsuarios()
        {
            var resultado = _context.Usuarios.ToList();
            return resultado;
        }

        public List<Usuario> ObtenerUsuariosCompras()
        {
            var resultado = _context.Usuarios.Where(r => r.Area == "COMPRAS").ToList();
            return resultado;
        }

        public List<Usuario> ObtenerUsuariosAutorizantes()
        {
            var resultado = _context.Usuarios
                .Where(r => r.IdUsuario == 3 || r.IdUsuario == 13 )
                .ToList();

            return resultado;
        }


    }
}
