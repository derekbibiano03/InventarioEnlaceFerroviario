using Inventario.Desktop.ViewModels.CatalogosViewModel;
using System.Windows.Controls;


namespace Inventario.Desktop.Views.UserControllers.Catalogos
{
    /// <summary>
    /// Lógica de interacción para GruposView.xaml
    /// </summary>
    public partial class GruposView : UserControl
    {
        public GruposView()
        {
            InitializeComponent();
            this.DataContext = new GruposViewModel();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
