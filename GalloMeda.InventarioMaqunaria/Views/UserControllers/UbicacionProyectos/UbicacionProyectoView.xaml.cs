using Inventario.Desktop.ViewModels.UbicacionProyectosViewModel;
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

namespace Inventario.Desktop.Views.UserControllers.UbicacionProyectos
{
    public partial class UbicacionProyectoView : UserControl
    {
        public UbicacionProyectoView()
        {
            InitializeComponent();
            this.DataContext = new UbicacionProyectoViewModel();
        }
    }
}
