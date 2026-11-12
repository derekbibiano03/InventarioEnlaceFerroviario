using Inventario.Desktop.ViewModels.EconomicosViewModel.Movimientos;
using System.Windows;

namespace Inventario.Desktop.Views.UserControllers.Economicos.Movimientos
{
    public partial class LlegadaMovimientoView : Window
    {
        public LlegadaMovimientoView(ReciboEquipoViewModel viewmodel)
        {
            InitializeComponent();
            DataContext = viewmodel; // 👈 Asigna el ViewModel aquí
            viewmodel.RequestClose += (success) =>
            {
                this.DialogResult = success;
                this.Close();
            };
        }
    }
}