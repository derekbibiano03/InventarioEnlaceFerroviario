using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Desktop.ViewModels.EconomicosViewModel.Servicios;
using Microsoft.Extensions.Configuration;
using System.Windows.Controls;
using Inventario.Core.DTOs;
using Inventario.Core.Services;
using Inventario.Core.Services.Adq_Serv.AdquisicionService;
using Inventario.Desktop.ViewModels.AdqServ;
using Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.OrdenesCompra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var ftpSettings = Options.Create(configuration.GetSection("ConfiguracionFtp").Get<FtpSettings>() ?? new FtpSettings());

            var context = new Data.Models.InventarioContext();
            var logsService = new LogsService(context);
            var economicosService = new CatalogoEconomicosService(context, logsService);

            var gestorarchivo = new GestorArchivosService(context, ftpSettings);
            var historialServicio = new HistorialServicioService(context, logsService, ftpSettings);
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
