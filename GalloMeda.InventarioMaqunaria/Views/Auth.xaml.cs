using Inventario.Core.Services.Auth;
using Inventario.Data.Models;
using Inventario.Desktop.ViewModels.Auth;
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
using System.Windows.Shapes;

namespace Inventario.Desktop.Views
{
    public partial class Auth : Window
    {
        public Auth()
        {
            InitializeComponent();
            string passwordIntroducida = TxtContrasena.Password;
        }
    }
}
