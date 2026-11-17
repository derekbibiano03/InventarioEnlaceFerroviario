using GalloMeda.InventarioMaqunaria;
using Inventario.Desktop.ViewModels.AdqServ;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.Requisiciones
{
    /// <summary>
    /// Lógica de interacción para RequisicionesPendientesView.xaml
    /// </summary>
    public partial class RequisicionesPendientesView : UserControl
    {
        public RequisicionesPendientesView()
        {
            InitializeComponent();

            // Solución: Obtenemos el ViewModel a través del contenedor global de dependencias de la aplicación
            using (var scope = App.ServiceProvider.CreateScope())
            {
                var viewModel = scope.ServiceProvider.GetRequiredService<ListaRequisicionesViewModel>();
                this.DataContext = viewModel;
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lógica opcional del evento o déjalo vacío si solo quieres que compile
        }
    }
}