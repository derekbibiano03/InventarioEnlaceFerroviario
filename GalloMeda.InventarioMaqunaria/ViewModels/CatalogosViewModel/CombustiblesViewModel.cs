using Inventario.Core.Services.Catalogos;
using Inventario.Data;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Inventario.Desktop.ViewModels.CatalogosViewModel
{
    public class CombustiblesViewModel
    {
        private readonly CatalogoCombustiblesService _combustiblesService;

        public ObservableCollection<CatalogoTiposCombustible> Combustibles { get; set; }

        public CombustiblesViewModel() 
        {
            var contexto = new InventarioContext();
            _combustiblesService = new CatalogoCombustiblesService(contexto);
            Combustibles = new ObservableCollection<CatalogoTiposCombustible>();
            CargarCombustibles();

        }

        public void CargarCombustibles()
        {
            Combustibles.Clear();

            var datos = _combustiblesService.ObtenerCombustibles();
            foreach (var dato in datos)
            {
                Combustibles.Add(dato);
            }
        }

    }
}
