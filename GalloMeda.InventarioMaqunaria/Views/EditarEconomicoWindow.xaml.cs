using Inventario.Desktop.ViewModels.EconomicosViewModel.InfoEconomico;
using System.Windows;

namespace Inventario.Desktop.Views
{
    /// <summary>
    /// Lógica de interacción para EditarEconomicoWindow.xaml
    /// </summary>
    public partial class EditarEconomicoWindow : Window
    {
        public EditarEconomicoWindow(string idEconomico)
        {
            InitializeComponent();

            var viewModel = new EditarEconomicoViewModel();

            // LÍNEA CRÍTICA: Asignar DataContext PRIMERO
            this.DataContext = viewModel;

            // LÍNEA CRÍTICA: Cargar datos DESPUÉS de que el DataContext ya existe
            viewModel.CargarDatosEconomico(idEconomico);

            if (viewModel.CloseAction == null)
            {
                viewModel.CloseAction = new System.Action<bool>((bool resultado) =>
                {
                    this.DialogResult = resultado;
                    this.Close();
                });
            }
        }
    }
}