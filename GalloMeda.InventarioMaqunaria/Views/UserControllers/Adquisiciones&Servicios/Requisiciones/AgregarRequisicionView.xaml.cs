using DocumentFormat.OpenXml.Drawing;
using GalloMeda.InventarioMaqunaria; // Asegúrate de tener este using para acceder a App.ServiceProvider si gustas, o leer la configuración directo
using Inventario.Core.Services;
using Inventario.Core.Services.Adq_Serv.AdquisicionService;
using Inventario.Core.Services.Auth;
using Inventario.Core.Services.Logs;
using Inventario.Desktop.ViewModels.AdqServ;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.Requisiciones
{
    public partial class AgregarRequisicionView : UserControl
    {
        public AgregarRequisicionView()
        {
            InitializeComponent();

            // Opción recomendada: Obtener el IConfiguration desde el contenedor global de la aplicación (App.ServiceProvider)
            // Si no usas App.ServiceProvider aquí, puedes construirlo de forma local como abajo:
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var context = new Data.Models.InventarioContext();
            var usuarios = new UsuariosService(context);
            var email = new EmailService(configuration); // <-- Aquí le pasamos la configuración requerida
            var adquisicionService = new AdquisicionService(context, email);
            var viewModel = new AgregarRequisicionViewModel(context, usuarios, adquisicionService, onRegistroRequisicionCompletado: () =>
            {
                // Aquí defines a qué pantalla te quieres redirigir.
                // Por ejemplo, si estás navegando dentro de un ContentControl en la ventana principal:
                if (this.Parent is ContentControl parentControl)
                {
                    parentControl.Content = new BuscarRequisicionView(); // Redirige de vuelta a la tabla principal
                }
            });

            this.DataContext = viewModel;
            this.Loaded += AgregarRequisicionView_Loaded;
        }

        private void AgregarRequisicionView_Loaded(object sender, RoutedEventArgs e)
        {
            // Desuscribir el evento para que la alerta solo se muestre una vez al abrir el control
            this.Loaded -= AgregarRequisicionView_Loaded;

            // Mostrar la alerta
            MessageBox.Show("¡Recordatorio: De ser necesario asegurate de colocar las medidas de las piezas que necesitas!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}