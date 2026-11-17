using Inventario.Core.DTOs;
using Inventario.Desktop.ViewModels.EconomicosViewModel.InfoEconomico;
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
using Microsoft.Extensions.Options;
using System.Windows.Controls;
using Inventario.Core.Services.Economicos;

namespace Inventario.Desktop.Views.UserControllers.Economicos
{
    /// <summary>
    /// Lógica de interacción para RegistrarArchivoView.xaml
    /// </summary>
    public partial class RegistrarArchivoView : UserControl
    {
        public RegistrarArchivoView()
        {
            InitializeComponent();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            var ftpSettings = Options.Create(configuration.GetSection("ConfiguracionFtp").Get<FtpSettings>() ?? new FtpSettings());
            var context = new Data.Models.InventarioContext();
            var gestorArchivos = new GestorArchivosService(context, ftpSettings);

            this.DataContext = new RegistrarArchivoViewModel(gestorArchivos);
        }
    }
}
