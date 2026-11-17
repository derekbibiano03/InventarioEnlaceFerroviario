using Inventario.Core.DTOs;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.Personal;
using Inventario.Desktop.ViewModels.EconomicosViewModel.InfoEconomico;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Economicos.InfoEconomicos
{
    public partial class ResguardosEconomicosView : UserControl
    {
        public ResguardosEconomicosView()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var ftpSettings = Options.Create(configuration.GetSection("ConfiguracionFtp").Get<FtpSettings>() ?? new FtpSettings());

            var context = new Data.Models.InventarioContext();
            var logsService = new LogsService(context);
            var empleadosService = new EmpleadoService(context);
            var economicosService = new CatalogoEconomicosService(context, logsService);
            var resguardoService = new ResguardosService(context, logsService);
            var gestorArchivos = new GestorArchivosService(context, ftpSettings);
            this.DataContext = new ResguardosEconomicosViewModel(context, empleadosService, economicosService, resguardoService, logsService, gestorArchivos);
            InitializeComponent();
        }
    }
}
