using Inventario.Core.DTOs;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Desktop.ViewModels.EconomicosViewModel.Movimientos;
using Microsoft.Extensions.Configuration;
using System.Windows.Controls;
using Microsoft.Extensions.Options; 

namespace Inventario.Desktop.Views.UserControllers.Economicos
{
    public partial class RealizarMovimientoView : UserControl
    {
        public RealizarMovimientoView()
        {
            InitializeComponent();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var context = new Data.Models.InventarioContext();
            var logsService = new LogsService(context);
            var ubicacionService = new UbicacionProyeectoService(context);
            var ftpSettings = Options.Create(configuration.GetSection("ConfiguracionFtp").Get<FtpSettings>() ?? new FtpSettings());
            var gestorArchivosService = new GestorArchivosService(context, ftpSettings);
            var economicosService = new CatalogoEconomicosService (context, logsService);
            var realizarMovimientosService = new RealizarMovimientosService(context, logsService, ftpSettings);
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
