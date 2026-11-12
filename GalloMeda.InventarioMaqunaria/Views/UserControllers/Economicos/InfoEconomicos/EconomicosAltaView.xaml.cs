using Inventario.Core.Services.Catalogos;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.Personal;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Desktop.ViewModels.EconomicosViewModel.InfoEconomico;
using System.Windows.Controls;
namespace Inventario.Desktop.Views.UserControllers.Economicos
{
    public partial class EconomicosAltaView : UserControl
    {
        public EconomicosAltaView()
        {
            InitializeComponent();
            var context = new Data.Models.InventarioContext();
            var logsService = new LogsService(context);
            var marcasServicio = new CatalogoMarcasService(context);
            var tipoEquipoServicio = new CatalogoTiposEquipoService(context);
            var grupoServicio = new CatalogoGruposService(context);
            var combusServicio = new CatalogoCombustiblesService(context);
            var pyaServicio = new ProAdminService(context);
            var ubicacionService = new UbicacionProyeectoService(context);
            var empleadosService = new EmpleadoService(context);
            var economicosService = new CatalogoEconomicosService(context, logsService);

            // Pasamos la acción de redirección en el último parámetro del constructor
            var viewModel = new EconomicosAltaViewModel(
                marcasServicio, tipoEquipoServicio, grupoServicio,
                combusServicio, pyaServicio, ubicacionService, empleadosService,
                economicosService,
                onRegistroCompletado: () =>
                {
                    // Aquí defines a qué pantalla te quieres redirigir.
                    // Por ejemplo, si estás navegando dentro de un ContentControl en la ventana principal:
                    if (this.Parent is ContentControl parentControl)
                    {
                        parentControl.Content = new EconomicosView(); // Redirige de vuelta a la tabla principal
                    }
                }
            );

            this.DataContext = viewModel;
        }
    }
}