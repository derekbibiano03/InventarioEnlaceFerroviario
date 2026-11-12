using Inventario.Core.Services.Catalogos;
using Inventario.Data;
using Inventario.Data.Models;
using System.Collections.ObjectModel;

namespace Inventario.Desktop.ViewModels.CatalogosViewModel
{
    public class EstatusViewModel
    {

        private readonly CatalogoEstatusService _estatusService;

        public ObservableCollection<CatalogoEstatus> Estatus { get; set; }

        public EstatusViewModel() 
        {
            var contexto = new InventarioContext();
            _estatusService = new CatalogoEstatusService(contexto);
            Estatus = new ObservableCollection<CatalogoEstatus>();
            CargarEstatus();
        }

        public void CargarEstatus()
        {
            Estatus.Clear();

            var datos = _estatusService.ObtenerEstatus();
            foreach (var dato in datos)
            {
                Estatus.Add(dato);
            }
        }
    }
}
