using Inventario.Desktop.ViewModels.EconomicosViewModel.Servicios;
using System.Windows;


namespace Inventario.Desktop.Views
{
    
    public partial class EditarServicioView : Window
    {
        public EditarServicioView(EditarServicioViewModel viewModel)
        {
            InitializeComponent();

            // Asigna el DataContext inyectado a la ventana
            DataContext = viewModel;

            // Suscripción al evento para cerrar la ventana cuando el guardado sea exitoso
            viewModel.RequestClose += (success) =>
            {
                this.DialogResult = success;
                this.Close();
            };
        }
    }
}
