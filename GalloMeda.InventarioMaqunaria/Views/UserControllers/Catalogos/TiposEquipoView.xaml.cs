using Inventario.Desktop.ViewModels.CatalogosViewModel;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Catalogos
{

    public partial class TiposEquipoView : UserControl
    {
        public TiposEquipoView()
        {
            InitializeComponent();
            this.DataContext = new TiposEquipoViewModel();
        }
    }
}
