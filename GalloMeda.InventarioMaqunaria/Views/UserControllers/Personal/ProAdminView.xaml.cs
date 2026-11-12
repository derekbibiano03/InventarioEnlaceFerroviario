using Inventario.Desktop.ViewModels.Personal;
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

namespace Inventario.Desktop.Views.UserControllers.Personal
{
    /// <summary>
    /// Lógica de interacción para PrroAdminView.xaml
    /// </summary>
    public partial class ProAdminView : UserControl
    {
        public ProAdminView()
        {
            InitializeComponent();
            this.DataContext = new ProAdminViewModel();
        }
    }
}
