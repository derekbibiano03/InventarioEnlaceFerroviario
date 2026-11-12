using Inventario.Core.Services.Personal;
using Inventario.Data;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Inventario.Desktop.ViewModels.Personal
{
    public class OperadoresViewModel
    {
        private readonly EmpleadoService _empleadoService;
        public ObservableCollection<Empleado> Operadores { get; set; }

        public OperadoresViewModel() 
        {
            var context = new InventarioContext();
            _empleadoService = new EmpleadoService(context);
            Operadores = new ObservableCollection<Empleado>();
            CargarOperadores();
        }

        public void CargarOperadores() 
        {
            var resultado = _empleadoService.ObtenerOperadoresConFechaNoNula();

            // Se limpian e insertan los elementos en la ObservableCollection
            Operadores.Clear();
            foreach (var operador in resultado)
            {
                Operadores.Add(operador);
            }
        }
    }
}
