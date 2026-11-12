using Inventario.Desktop.ViewModels.EconomicosViewModel.InfoEconomico;
using System.Windows.Controls;
namespace Inventario.Desktop.Views.UserControllers.Economicos
{
    public partial class EconomicosView : UserControl
    {
        public EconomicosView()
        {
            InitializeComponent();

            this.DataContext = new EconomicosViewModel();
        }

    }
}
