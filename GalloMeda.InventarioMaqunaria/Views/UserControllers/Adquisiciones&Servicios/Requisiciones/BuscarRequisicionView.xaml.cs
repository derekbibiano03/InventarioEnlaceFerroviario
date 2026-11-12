using Inventario.Core.Services;
using Inventario.Core.Services.Adq_Serv.AdquisicionService;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Desktop.ViewModels.AdqServ;
using Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.OrdenesCompra;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.Requisiciones
{
    /// <summary>
    /// Lógica de interacción para BuscarRequisicionView.xaml
    /// </summary>
    public partial class BuscarRequisicionView : UserControl
    {
        public BuscarRequisicionView()
        {
            InitializeComponent();

            // Construir la configuración local para el servicio de correo
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Instanciar las dependencias necesarias para el ViewModel
            var context = new Data.Models.InventarioContext();
            var emailService = new EmailService(configuration);
            var adquisicionService = new AdquisicionService(context, emailService);

            // Asignar el ViewModel pasándole el servicio requerido
            var viewModel = new BuscarRequisicionesViewModel(adquisicionService, context, onRecargarBusqueda: () =>
            {
                if (this.Parent is ContentControl parentControl)
                {
                    parentControl.Content = new BuscarRequisicionView(); // Redirige de vuelta a la tabla principal
                }
            });

            // Configurar la acción para navegar a OrdenCompraView pasando el ID de la requisición
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