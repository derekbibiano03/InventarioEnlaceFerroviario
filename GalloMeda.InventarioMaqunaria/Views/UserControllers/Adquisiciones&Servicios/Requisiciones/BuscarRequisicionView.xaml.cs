using GalloMeda.InventarioMaqunaria;
using Inventario.Desktop.ViewModels.AdqServ;
using Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.OrdenesCompra;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.Requisiciones
{
    public partial class BuscarRequisicionView : UserControl
    {
        public BuscarRequisicionView()
        {
            InitializeComponent();

            // Resolvemos el ViewModel de forma segura a través del contenedor global de dependencias
            using (var scope = App.ServiceProvider.CreateScope())
            {
                var viewModel = scope.ServiceProvider.GetRequiredService<BuscarRequisicionesViewModel>();

                // Configuramos la acción de recarga de la vista
                viewModel.GenerarOCAction = (idRequisicion) =>
                {
                    if (this.Parent is ContentControl parentControl)
                    {
                        parentControl.Content = new OrdenCompraView(idRequisicion);
                    }
                };

                this.DataContext = viewModel;
            }
        }
    }
}