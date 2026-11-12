using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Inventario.Core.Services.Personal;
using Inventario.Data;
using Inventario.Data.Models;

namespace Inventario.Desktop.ViewModels.Personal
{
    public class ProAdminViewModel
    {
        private readonly ProAdminService _proAdminService;

        public ObservableCollection<CatalogoPya> PYA { get; set; }

        public ProAdminViewModel() 
        {
            var contexto = new InventarioContext();
            _proAdminService = new ProAdminService(contexto);
            PYA = new ObservableCollection<CatalogoPya>();
            CargarPYA();
        }

        public void CargarPYA()
        {
            PYA.Clear();
            var datospya = _proAdminService.ObtenerPYA();
            foreach (var dato in datospya)
            {
                PYA.Add(dato);
            } 
        }
    }
}
