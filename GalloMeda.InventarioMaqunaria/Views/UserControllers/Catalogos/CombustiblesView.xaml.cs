using Inventario.Desktop.ViewModels.CatalogosViewModel;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Catalogos
{
    public partial class CombustiblesView : UserControl
    {
        public CombustiblesView()
        {
            InitializeComponent();
            this.DataContext = new CombustiblesViewModel();
        }
    }
}
