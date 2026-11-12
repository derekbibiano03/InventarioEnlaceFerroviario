using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Desktop.ViewModels.EconomicosViewModel.Servicios;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Economicos
{
    /// <summary>
    /// Lógica de interacción para HistorialServicioView.xaml
    /// </summary>
    public partial class HistorialServicioView : UserControl
    {
        public HistorialServicioView()
        {
            InitializeComponent();
            var context = new Data.Models.InventarioContext();
            var logsService = new LogsService(context);
            var economicosService = new CatalogoEconomicosService(context, logsService);
            var gestorarchivo = new GestorArchivosService(context);
            var historialServicio = new HistorialServicioService(context, logsService);
            var ubicacionService = new UbicacionProyeectoService(context);
            var viewModel = new HistorialServicioViewModel(
                economicosService, 
                historialServicio, 
                gestorarchivo, 
                ubicacionService,
                onRegistroCompletado: () =>
                {
                    if (this.Parent is ContentControl parentControl)
                    {
                        parentControl.Content = new HistorialServicioView();
                    }
                });
            this.DataContext = viewModel;
        }
    }
}
