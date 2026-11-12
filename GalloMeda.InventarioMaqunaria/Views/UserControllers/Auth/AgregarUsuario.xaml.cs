using Inventario.Core.Services.Auth;
using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Inventario.Desktop.ViewModels.Auth;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Auth
{
    public partial class AgregarUsuario : UserControl
    {
        public AgregarUsuario()
        {
           InitializeComponent();
           var context = new InventarioContext();

            var logsService = new LogsService(context);

           var authService = new AutenticacionService(context, logsService);

           this.DataContext = new AgregarUsuarioViewModel(authService);
        }
    }
}
