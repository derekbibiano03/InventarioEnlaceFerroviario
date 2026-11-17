using Inventario.Core.DTOs; // Necesario para FtpSettings
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Desktop.ViewModels.EconomicosViewModel.Movimientos;
using Microsoft.Extensions.Configuration; // Necesario para ConfigurationBuilder
using Microsoft.Extensions.Options; // Necesario para Options.Create
using System;
using System.IO;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Economicos
{
    /// <summary>
    /// Lógica de interacción para HistorialMovimientosView.xaml
    /// </summary>
    public partial class HistorialMovimientosView : UserControl
    {
        public HistorialMovimientosView()
        {
            InitializeComponent();

            // Construir la configuración para leer el appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var context = new Data.Models.InventarioContext();
            var logsService = new LogsService(context);
            var ubicacionService = new UbicacionProyeectoService(context);

            // Creamos las opciones de FTP requeridas por GestorArchivosService
            var ftpSettings = Options.Create(configuration.GetSection("ConfiguracionFtp").Get<FtpSettings>() ?? new FtpSettings());
            var gestorArchivosService = new GestorArchivosService(context, ftpSettings);

            var economicosService = new CatalogoEconomicosService(context, logsService);
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