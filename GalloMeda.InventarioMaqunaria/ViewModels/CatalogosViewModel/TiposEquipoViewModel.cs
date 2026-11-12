using Inventario.Core.Services.Catalogos;
using Inventario.Data;
using Inventario.Data.Models;
using System.Collections.ObjectModel;

namespace Inventario.Desktop.ViewModels.CatalogosViewModel
{
    public class TiposEquipoViewModel
    {
        private readonly CatalogoTiposEquipoService _tipoEquipoService;

        public ObservableCollection<CatalogoTiposEquipo> TipoEquipo { get; set; }

        public TiposEquipoViewModel()
        {
            var contexto = new InventarioContext();
            _tipoEquipoService = new CatalogoTiposEquipoService(contexto);
            TipoEquipo = new ObservableCollection<CatalogoTiposEquipo>();
            CargarTiposEq();
        }

        public void CargarTiposEq()
        {
            TipoEquipo.Clear();

            var datos = _tipoEquipoService.ObtenerTiposEq();

            foreach (var dato in datos)
            {

                TipoEquipo.Add(dato);

            }
        }
    }
}
