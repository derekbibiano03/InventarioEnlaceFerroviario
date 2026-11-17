using Inventario.Data.Models;

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
