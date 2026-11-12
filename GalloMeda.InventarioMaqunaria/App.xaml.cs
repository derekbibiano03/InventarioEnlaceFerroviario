using AutoUpdaterDotNET;
using Inventario.Core.Services;
using Inventario.Core.Services.Auth;
using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Inventario.Desktop.ViewModels.Auth;
using Inventario.Desktop.Views;
using InventarioMaquinaria.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace GalloMeda.InventarioMaqunaria
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public static ISessionService Session { get; private set; } = new SessionService();
        public static string ConnectionString { get; set; } = string.Empty;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Exception ex = (Exception)args.ExceptionObject;
                MessageBox.Show($"Error no controlado en la aplicación:\n\n{ex.Message}\n\n{ex.InnerException?.Message}", "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string configPath = Path.Combine(basePath, "appsettings.json");

                // 1. Validar que el archivo exista antes de intentar leerlo
                if (!File.Exists(configPath))
                {
                    throw new FileNotFoundException("No se encontró el archivo de configuración 'appsettings.json' en la carpeta del programa.");
                }

                // 2. Cargar configuración
                var builder = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                IConfiguration configuration = builder.Build();

                // 3. Configurar AutoUpdater (Público desde GitHub Releases)
                string? githubToken = configuration["GitHubUpdate:Token"];
                string? updateXmlUrl = configuration["GitHubUpdate:XmlUrl"];

                if (!string.IsNullOrWhiteSpace(githubToken) && !string.IsNullOrWhiteSpace(updateXmlUrl))
                {
                    var auth = new BasicAuthentication("derekbibiano03", githubToken);
                    AutoUpdater.BasicAuthXML = auth;
                    AutoUpdater.BasicAuthDownload = auth;
                    AutoUpdater.ApplicationExitEvent += AutoUpdater_ApplicationExitEvent;
                    AutoUpdater.Start(updateXmlUrl);
                }

                // 4. Configurar Base de Datos y Servicios
                var serviceCollection = new ServiceCollection();
                var connectionString = configuration.GetConnectionString("InventarioConnection");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("La cadena de conexión 'InventarioConnection' está vacía en appsettings.json.");
                }

                serviceCollection.AddDbContext<InventarioContext>(options =>
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                );

                serviceCollection.AddScoped<LogsService>();
                serviceCollection.AddScoped<AutenticacionService>();
                serviceCollection.AddSingleton(configuration);
                serviceCollection.AddScoped<EmailService>();

                ServiceProvider = serviceCollection.BuildServiceProvider();

                // 5. Iniciar ventana de Login
                using (var scope = ServiceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<InventarioContext>();
                    var logsService = new LogsService(context);
                    var authService = new AutenticacionService(context, logsService);
                    var loginVM = new LoginViewModel(authService, logsService);
                    var loginWindow = new Auth();

                    loginWindow.DataContext = loginVM;
                    this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                    bool? result = loginWindow.ShowDialog();

                    if (result == true && loginVM.IsAutenticado)
                    {
                        string usuarioConfirmado = App.Session.Username;
                        int idRolUsuario = App.Session.IdRol;
                        string NombreCompleto = App.Session.NombreCompleto;
                        var mainWindow = new MainWindow(usuarioConfirmado, idRolUsuario, NombreCompleto);
                        this.MainWindow = mainWindow;
                        this.ShutdownMode = ShutdownMode.OnMainWindowClose;
                        mainWindow.Show();
                    }
                    else
                    {
                        this.Shutdown();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar la aplicación:\n\n{ex.Message}\n\nDetalle: {ex.InnerException?.Message}", "Error de Inicialización", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Shutdown();
            }
        }

        private void AutoUpdater_ApplicationExitEvent()
        {
            Application.Current.Shutdown();
        }
    }
}