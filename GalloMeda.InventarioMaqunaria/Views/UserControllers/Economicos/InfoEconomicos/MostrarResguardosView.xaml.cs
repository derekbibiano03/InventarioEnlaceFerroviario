using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.Personal;
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
    /// Lógica de interacción para MostrarResguardosView.xaml
    /// </summary>
    public partial class MostrarResguardosView : UserControl
    {
        public MostrarResguardosView()
        {
            var context = new Data.Models.InventarioContext();
            var logsService = new LogsService(context);
            var empleadosService = new EmpleadoService(context);
            var economicosService = new CatalogoEconomicosService(context, logsService);
            var resguardoService = new ResguardosService(context, logsService);
            this.DataContext = new ResguardosEconomicosViewModel(context, empleadosService, economicosService, resguardoService, logsService);
            InitializeComponent();
        }
    }
}
