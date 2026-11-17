using GalloMeda.InventarioMaqunaria;
using Inventario.Desktop.ViewModels.AdqServ;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.Requisiciones
{
    public partial class AgregarRequisicionView : UserControl
    {
        public AgregarRequisicionView()
        {
            InitializeComponent();
            using (var scope = App.ServiceProvider.CreateScope())
            {
                var viewModel = scope.ServiceProvider.GetRequiredService<AgregarRequisicionViewModel>();

                viewModel.OnRegistroRequisicionCompletado = () =>
                {
                    if (this.Parent is ContentControl parentControl)
                    {
                        parentControl.Content = new BuscarRequisicionView();
                    }
                };

                this.DataContext = viewModel;
            }

            this.Loaded += AgregarRequisicionView_Loaded;
        }

        private void AgregarRequisicionView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= AgregarRequisicionView_Loaded;
            MessageBox.Show("¡Recordatorio: De ser necesario asegurate de colocar las medidas de las piezas que necesitas!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}