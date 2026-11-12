using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Desktop.ViewModels.EconomicosViewModel.Servicios;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Economicos
{
    /// <summary>
    /// Lógica de interacción para AgregarServicioView.xaml
    /// </summary>
    public partial class AgregarServicioView : UserControl
    {
        public AgregarServicioView()
        {
            InitializeComponent();
            var context = new Data.Models.InventarioContext();
            var logsService = new LogsService(context);
            var economicosService = new CatalogoEconomicosService(context, logsService);
            var historialServiciosService = new HistorialServicioService(context, logsService);
            var ubicacionesService = new UbicacionProyeectoService(context);
            var gestorArchivosService = new GestorArchivosService(context);

            var viewModel = new HistorialServicioViewModel(
                economicosService, 
                historialServiciosService, 
                gestorArchivosService, 
                ubicacionesService,
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
