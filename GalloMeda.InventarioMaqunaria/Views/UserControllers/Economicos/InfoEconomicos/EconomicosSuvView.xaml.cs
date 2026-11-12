using Inventario.Desktop.ViewModels.EconomicosViewModel.InfoEconomico;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inventario.Desktop.Views.UserControllers.Economicos.InfoEconomicos
{
    /// <summary>
    /// Lógica de interacción para EconomicosSuvView.xaml
    /// </summary>
    public partial class EconomicosSuvView : UserControl
    {
        public EconomicosSuvView()
        {
            InitializeComponent();
            this.DataContext = new EconomicosSuvViewModel();
        }
    }
}
