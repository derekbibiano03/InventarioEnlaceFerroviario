using Inventario.Desktop.ViewModels.UbicacionProyectosViewModel;
using System.Windows;
using Inventario.Core.DTOs;
using Inventario.Core.Services.Economicos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Inventario.Desktop.Views
{
    public partial class DetallesWindow : Window
    {
        public DetallesWindow(string idEconomico)
        {
            InitializeComponent();
            var context = new Data.Models.InventarioContext();
            var configuration = new ConfigurationBuilder()
              .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .Build();
            var ftpSettings = Options.Create(configuration.GetSection("ConfiguracionFtp").Get<FtpSettings>() ?? new FtpSettings());
            var gestorArchivos = new GestorArchivosService(context, ftpSettings);
            this.DataContext = new DetallesViewModel(idEconomico, gestorArchivos);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
