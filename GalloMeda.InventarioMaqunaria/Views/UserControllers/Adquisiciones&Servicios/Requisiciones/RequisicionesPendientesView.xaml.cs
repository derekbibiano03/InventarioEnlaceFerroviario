using Inventario.Core.Services;
using Inventario.Core.Services.Adq_Serv.AdquisicionService;
using Inventario.Desktop.ViewModels.AdqServ;
using Microsoft.Extensions.Configuration;
using System;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.Requisiciones
{
    /// <summary>
    /// Lógica de interacción para RequisicionesPendientesView.xaml
    /// </summary>
    public partial class RequisicionesPendientesView : UserControl
    {
        public RequisicionesPendientesView()
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
            this.DataContext = new ListaRequisicionesViewModel(adquisicionService);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lógica opcional del evento o déjalo vacío si solo quieres que compile
        }
    }
}