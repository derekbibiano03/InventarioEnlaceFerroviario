using Inventario.Core.Services.Personal;
using Inventario.Data;
using Inventario.Data.Models;
using Inventario.Desktop.ViewModels.EconomicosViewModel;
using System.Collections.ObjectModel;

namespace Inventario.Desktop.ViewModels.Personal
{
    public class EncargadosViewModel
    {
        private readonly EmpleadoService _empleadosService;
        public ObservableCollection<Empleado> Encargados { get; set; }

        public EncargadosViewModel()
        {
            var context = new InventarioContext();
            _empleadosService = new EmpleadoService(context);
            Encargados = new ObservableCollection<Empleado>();
            CargarResponsables();
        }

        public void CargarResponsables()
        { 
            Encargados.Clear();
            var rolesPermitidos = new List<int> { 2, 3, 4, 5, 6, 7 };
            var datosEncar = _empleadosService.ObtenerResponsables(rolesPermitidos);
            foreach (var dato in datosEncar)
            {
                Encargados.Add(dato);
            }
        }
    }
}
