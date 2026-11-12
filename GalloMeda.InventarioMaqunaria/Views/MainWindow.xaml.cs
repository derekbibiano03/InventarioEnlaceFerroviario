using DocumentFormat.OpenXml.CustomProperties;
using GalloMeda.InventarioMaqunaria;
using Inventario.Desktop.Views.UserControllers;
using Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.Requisiciones;
using Inventario.Desktop.Views.UserControllers.Auth;
using Inventario.Desktop.Views.UserControllers.Catalogos;
using Inventario.Desktop.Views.UserControllers.Economicos;
using Inventario.Desktop.Views.UserControllers.Economicos.InfoEconomicos;
using Inventario.Desktop.Views.UserControllers.Personal;
using Inventario.Desktop.Views.UserControllers.Proveedores;
using Inventario.Desktop.Views.UserControllers.UbicacionProyectos;
using System.ComponentModel;
using System;
using System.Reflection;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Inventario.Desktop.ViewModels.AdqServ.OrdenesCompra;
using Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.OrdenesCompra;

namespace Inventario.Desktop.Views
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _usuarioLogueado = string.Empty;
        private string _nombreUsuarioCompleto = string.Empty;
        private int _idRolUsuario;

        public string UsuarioLogueado
        {
            get => _usuarioLogueado;
            set
            {
                _usuarioLogueado = value;
                OnPropertyChanged();
            }
        }

        public string NombreUsuarioCompleto
        {
            get => _nombreUsuarioCompleto;
            set
            {
                _nombreUsuarioCompleto = value;
                OnPropertyChanged();
            }
        }

        public int IdRolUsuario
        {
            get => _idRolUsuario;
            set { _idRolUsuario = value; OnPropertyChanged(); }
        }

        public MainWindow(string username, int idRolUsuario, string nombreCompleto)
        {
            InitializeComponent();
            ContenedorPrincipal.Content = new EconomicosView();
            string textoPanel = "INVENTARIO DE MAQUINARIA";

            this.DataContext = this;
            this.UsuarioLogueado = username;
            this.IdRolUsuario = idRolUsuario;
            this.NombreUsuarioCompleto = nombreCompleto;
            txtPanel.Text = textoPanel;
        }

        private void BtnColapsar_Click(object sender, RoutedEventArgs e)
        {
            ColumnaMenu.Width = new GridLength(0);
            BtnMostrarMenu.Visibility = Visibility.Visible;
        }

        private void BtnMostrar_Click(object sender, RoutedEventArgs e)
        {
            ColumnaMenu.Width = new GridLength(220);
            BtnMostrarMenu.Visibility = Visibility.Collapsed;
        }

        private void BtnProductosMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton && boton.ContextMenu != null)
            {
                boton.ContextMenu.PlacementTarget = boton;
                boton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                boton.ContextMenu.IsOpen = true;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string textoPanel = "";
            if (sender is MenuItem itemPresionado)
            {
                string? ID_Interno = itemPresionado.Tag?.ToString();

                switch (ID_Interno)
                {
                    case "AddUser":
                        ContenedorPrincipal.Content = new AgregarUsuario();
                        textoPanel = "AGREGAR A UN NUEVO USUARIO";
                        break; 
                    case "ResguardoEconomico":
                        ContenedorPrincipal.Content = new ResguardosEconomicosView();
                        textoPanel = "NUEVO RESGUARDO";
                        break;
                    case "VerResguardosEconomico":
                        ContenedorPrincipal.Content = new MostrarResguardosView();
                        textoPanel = "VER TODOS LOS RESGUARDOS";
                        break;
                    case "VerInventario":
                        ContenedorPrincipal.Content = new EconomicosView();
                        textoPanel = "INVENTARIO DE MAQUINARIA";
                        break;
                    case "VerSuv":
                        ContenedorPrincipal.Content = new EconomicosSuvView();
                        textoPanel = "CAMIONETAS SUV";
                        break;
                    case "MoverEconomico":
                        ContenedorPrincipal.Content = new RealizarMovimientoView();
                        textoPanel = "REALIZAR UN MOVIMIENTO DE MAQUINARIA";
                        break;
                    case "AgregarEconomico":
                        ContenedorPrincipal.Content = new EconomicosAltaView();
                        textoPanel = "ALTA DE NUEVO ECONOMICO";
                        break;
                    case "AgregarArchivo":
                        ContenedorPrincipal.Content = new RegistrarArchivoView();
                        textoPanel = "ADJUNTAR DOCUMENTO O IMAGEN";
                        break;
                    case "HisMoverEconomico":
                        ContenedorPrincipal.Content = new HistorialMovimientosView();
                        textoPanel = "HISTORIAL DE MOVIMIENTOS";
                        break;

                    case "VerOperadores":
                        ContenedorPrincipal.Content = new OperadoresView();
                        textoPanel = "CATALOGO DE OPERADORES DE MAQUINARIA";
                        break;
                    case "VerEnMaq":
                        ContenedorPrincipal.Content = new EncargadosView();
                        textoPanel = "CATALOGO DE ENCARGADOS DE MAQUIINARIA";
                        break;
                    case "ProAdmin":
                        ContenedorPrincipal.Content = new ProAdminView();
                        textoPanel = "CATALOGO DE PROPIETARIOS Y ADMINISTRADORES DE MAQUINARIA";
                        break;

                    case "VerBrokers":
                        ContenedorPrincipal.Content = new BrokersView();
                        textoPanel = "CATALOGO DE BROKERS";
                        break;

                    case "CatCom":
                        ContenedorPrincipal.Content = new CombustiblesView();
                        textoPanel = "CATALOGO DE COMBUSTIBLES";
                        break;
                    case "CatTipEq":
                        ContenedorPrincipal.Content = new TiposEquipoView();
                        textoPanel = "CATALOGO DE TIPOS DE EQUIPOS";
                        break;
                    case "CatGrp":
                        ContenedorPrincipal.Content = new GruposView();
                        textoPanel = "CATALOGO DE GRUPOS";
                        break;
                    case "CatTiEs":
                        ContenedorPrincipal.Content = new EstatusView();
                        textoPanel = "CATALOGO DE ESTATUS";
                        break;
                    case "CatMarc":
                        ContenedorPrincipal.Content = new MarcasView();
                        textoPanel = "CATALOGO DE MARCAS";
                        break;

                    case "VerUbicaciones":
                        ContenedorPrincipal.Content = new UbicacionProyectoView();
                        textoPanel = "CATALOGO DE UBICACIONES";
                        break;
                    case "VerTramos":
                        ContenedorPrincipal.Content = new TramosView();
                        textoPanel = "CATALOGO DE TRAMOS";
                        break;
                    case "VerFrentes":
                        ContenedorPrincipal.Content = new FrentesView();
                        textoPanel = "CATALOGO DE FRENTES";
                        break;

                    case "AddReq":
                        ContenedorPrincipal.Content = new AgregarRequisicionView();
                        textoPanel = "AÑADIR NUEVA REQUISICION";
                        break;

                    case "HistServ":
                        ContenedorPrincipal.Content = new AgregarServicioView();
                        textoPanel = "NUEVO SERVICIOS REALIZADOS";
                        break;

                    case "VerServicios":
                        ContenedorPrincipal.Content = new HistorialServicioView();
                        textoPanel = "HISTORIAL DE SERVICIOS REALIZADOS";
                        break;
                    case "AutReq":
                        ContenedorPrincipal.Content = new RequisicionesPendientesView();
                        textoPanel = "AUTORIZACION DE REQUISICIONES";
                        break;
                    case "BuscReq":
                        ContenedorPrincipal.Content = new BuscarRequisicionView();
                        textoPanel = "AUTORIZACION DE REQUISICIONES";
                        break;
                    case "CatMaterial":
                        ContenedorPrincipal.Content = new CatalogoMaterialView();
                        textoPanel = "CATALOGO DE MATERIALES";
                        break;
                }
                txtPanel.Text = textoPanel;
            }
        }

        private async void ActualizarVersion(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Obtener la versión actual del ensamblado instalado (ej. 1.0.0.0)
                Version versionActual = Assembly.GetExecutingAssembly().GetName().Version;

                // 2. Consultar la última versión desde la API de GitHub Releases
                string urlApi = "https://api.github.com/repos/derekbibiano03/Inventario/releases/latest";

                using (var client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "InventarioDesktopApp");

                    var respuesta = await client.GetAsync(urlApi);
                    if (!respuesta.IsSuccessStatusCode)
                    {
                        MessageBox.Show("No se pudo verificar la actualización con el servidor.", "Error de red", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var jsonContent = await respuesta.Content.ReadAsStringAsync();

                    string tagName = ExtraerTagFromJson(jsonContent);
                    if (string.IsNullOrEmpty(tagName))
                    {
                        MessageBox.Show("No se encontró una versión válida en las liberaciones.", "Actualización", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    // 3. Limpiar la 'v' inicial y dividir el tag (Ej: "1.0.20261012153439")
                    string versionLimia = tagName.TrimStart('v', 'V');
                    string[] partesRemotas = versionLimia.Split('.');

                    if (partesRemotas.Length < 3)
                    {
                        MessageBox.Show($"La versión remota ({tagName}) no tiene un formato compatible.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Extraer componentes de forma segura usando 'long' para el timestamp
                    int majorRemoto = int.Parse(partesRemotas[0]);
                    int minorRemoto = int.Parse(partesRemotas[1]);
                    long timestampRemoto = long.Parse(partesRemotas[2]);

                    int majorActual = versionActual.Major;
                    int minorActual = versionActual.Minor;
                    long timestampActual = versionActual.Build > 0 ? versionActual.Build : 0;

                    // 4. Lógica de comparación personalizada
                    bool hayNuevaVersion = false;

                    if (majorRemoto > majorActual)
                    {
                        hayNuevaVersion = true;
                    }
                    else if (majorRemoto == majorActual && minorRemoto > minorActual)
                    {
                        hayNuevaVersion = true;
                    }
                    else if (majorRemoto == majorActual && minorRemoto == minorActual)
                    {
                        // Si el timestamp del servidor es mayor al build actual, hay actualización
                        hayNuevaVersion = timestampRemoto > timestampActual;
                    }

                    if (hayNuevaVersion)
                    {
                        var resultado = MessageBox.Show(
                            $"Hay una nueva versión disponible ({tagName}). ¿Deseas actualizar ahora?",
                            "Actualización disponible",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Information);

                        if (resultado == MessageBoxResult.Yes)
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = $"https://github.com/derekbibiano03/Inventario/releases/latest",
                                UseShellExecute = true
                            });
                        }
                    }
                    else
                    {
                        MessageBox.Show("Tu aplicación ya está actualizada a la versión más reciente.", "Sin actualizaciones", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al buscar actualizaciones: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string ExtraerTagFromJson(string json)
        {
            string marcador = "\"tag_name\":\"";
            int indiceInicio = json.IndexOf(marcador);
            if (indiceInicio == -1) return string.Empty;

            indiceInicio += marcador.Length;
            int indiceFin = json.IndexOf("\"", indiceInicio);
            if (indiceFin == -1) return string.Empty;

            return json.Substring(indiceInicio, indiceFin - indiceInicio);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}