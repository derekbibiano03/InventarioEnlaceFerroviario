using GalloMeda.InventarioMaqunaria;
using Inventario.Desktop.ViewModels.EconomicosViewModel.Movimientos;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Economicos
{
    public partial class RealizarMovimientoView : UserControl
    {
        public RealizarMovimientoView()
        {
            InitializeComponent();

            // Usamos el contenedor global para que inyecte automáticamente el contexto,
            // los logs, las ubicaciones, el FTP y sus credenciales correctas sin errores.
            using (var scope = App.ServiceProvider.CreateScope())
            {
                var viewModel = scope.ServiceProvider.GetRequiredService<RealizarMovimientoViewModel>();

                viewModel.OnRegistroMovimientoCompletado = () =>
                {
                    if (this.Parent is ContentControl parentControl)
                    {
                        parentControl.Content = new HistorialMovimientosView();
                    }
                };

                this.DataContext = viewModel;
            }
        }
    }
}