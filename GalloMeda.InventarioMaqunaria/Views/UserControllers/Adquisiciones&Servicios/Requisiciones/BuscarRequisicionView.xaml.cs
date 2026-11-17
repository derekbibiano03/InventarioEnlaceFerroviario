using Inventario.Core.DTOs;
using Inventario.Core.Services;
using Inventario.Core.Services.Adq_Serv.AdquisicionService;
using Inventario.Desktop.ViewModels.AdqServ;
using Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.OrdenesCompra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options; 
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.Requisiciones
{
    public partial class BuscarRequisicionView : UserControl
    {
        public BuscarRequisicionView()
        {
            InitializeComponent();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var context = new Data.Models.InventarioContext();
            var emailService = new EmailService(configuration);

            var ftpSettings = Options.Create(configuration.GetSection("ConfiguracionFtp").Get<FtpSettings>() ?? new FtpSettings());

            var adquisicionService = new AdquisicionService(context, emailService, ftpSettings);

            var viewModel = new BuscarRequisicionesViewModel(adquisicionService, context, onRecargarBusqueda: () =>
            {
                if (this.Parent is ContentControl parentControl)
                {
                    parentControl.Content = new BuscarRequisicionView(); 
                }
            });

            viewModel.GenerarOCAction = (idRequisicion) =>
            {
                if (this.Parent is ContentControl parentControl)
                {
                    parentControl.Content = new OrdenCompraView(idRequisicion);
                }
            };

            this.DataContext = viewModel;
        }
    }
}