using Inventario.Core.DTOs;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Desktop.ViewModels.EconomicosViewModel.Servicios;
using Microsoft.Extensions.Configuration;
using System.Windows.Controls;
using Microsoft.Extensions.Options;

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

            var configuration = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .Build();

            var ftpSettings = Options.Create(configuration.GetSection("ConfiguracionFtp").Get<FtpSettings>() ?? new FtpSettings());

            var context = new Data.Models.InventarioContext();
            var logsService = new LogsService(context);
            var economicosService = new CatalogoEconomicosService(context, logsService);
            var historialServiciosService = new HistorialServicioService(context, logsService, ftpSettings);
            var ubicacionesService = new UbicacionProyeectoService(context);

            var gestorArchivosService = new GestorArchivosService(context, ftpSettings);

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
