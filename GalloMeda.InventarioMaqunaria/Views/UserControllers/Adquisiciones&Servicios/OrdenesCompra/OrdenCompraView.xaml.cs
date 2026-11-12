using Inventario.Desktop.ViewModels.AdqServ.OrdenesCompra;
using System;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.OrdenesCompra
{
    /// <summary>
    /// Lógica de interacción para OrdenCompraView.xaml
    /// </summary>
    public partial class OrdenCompraView : UserControl
    {
        // Constructor que recibe el ID de la requisición seleccionada
        public OrdenCompraView(string idRequisicion)
        {
            InitializeComponent();
            var context = new Data.Models.InventarioContext();

            // Pasamos el contexto y el ID de la requisición al ViewModel
            var viewModel = new OrdenCompraViewModel(context, idRequisicion);
            this.DataContext = viewModel;
        }
    }
}