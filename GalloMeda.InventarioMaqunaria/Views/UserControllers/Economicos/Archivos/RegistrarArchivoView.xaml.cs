using Inventario.Desktop.ViewModels.EconomicosViewModel.InfoEconomico;
using System.Windows.Controls;

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
            this.DataContext = new RegistrarArchivoViewModel();
        }
    }
}
