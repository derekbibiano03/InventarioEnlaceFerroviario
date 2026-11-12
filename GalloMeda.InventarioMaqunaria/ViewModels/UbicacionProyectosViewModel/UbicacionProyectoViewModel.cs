using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Data;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text;

namespace Inventario.Desktop.ViewModels.UbicacionProyectosViewModel
{
    public class UbicacionProyectoViewModel
    {
        private readonly UbicacionProyeectoService _ubicacionService;

        public ObservableCollection<CatalogoUbicacionesProyecto> Ubicaciones { get; set; }

        public UbicacionProyectoViewModel() 
        { 
            var contexto = new InventarioContext();
            _ubicacionService = new UbicacionProyeectoService(contexto);
            Ubicaciones = new ObservableCollection<CatalogoUbicacionesProyecto>();
            CargarUbicaciones();
        }

        public void CargarUbicaciones()
        { 
            Ubicaciones.Clear();
            var datosUbi = _ubicacionService.ObtenerUbicaciones();
            foreach(var datou in datosUbi)
            {
                Ubicaciones.Add(datou);
            }
        }
    }
}
