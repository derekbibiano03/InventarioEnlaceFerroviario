using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Desktop.ViewModels.EconomicosViewModel.Movimientos;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Economicos
{
    /// <summary>
    /// Lógica de interacción para RealizarMovimientoView.xaml
    /// </summary>
    public partial class RealizarMovimientoView : UserControl
    {
        public RealizarMovimientoView()
        {
            InitializeComponent();
            var context = new Data.Models.InventarioContext();
            var logsService = new LogsService(context);
            var ubicacionService = new UbicacionProyeectoService(context);
            var gestorArchivosService = new GestorArchivosService(context);
            var economicosService = new CatalogoEconomicosService (context, logsService);
            var realizarMovimientosService = new RealizarMovimientosService(context, logsService);
            var viewModel = new RealizarMovimientoViewModel(ubicacionService, economicosService, realizarMovimientosService, gestorArchivosService, onRegistroMovimientoCompletado: () =>
            {
                if (this.Parent is ContentControl parentControl)
                {
                    parentControl.Content = new HistorialMovimientosView();
                }
            });
            this.DataContext = viewModel;
        }
    }
}
