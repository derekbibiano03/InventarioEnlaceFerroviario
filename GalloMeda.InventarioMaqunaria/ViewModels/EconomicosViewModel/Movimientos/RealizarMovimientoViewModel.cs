using GalloMeda.InventarioMaqunaria;
using Inventario.Core;
using Inventario.Core.DTOs;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Data.Models;
using Inventario.Desktop.ViewModels.EconomicosViewModel.Servicios;
using Inventario.Desktop.Views;
using Inventario.Desktop.Views.UserControllers.Economicos.Movimientos;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.EconomicosViewModel.Movimientos
{
    public class RealizarMovimientoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private DateTime? _fechaMovimiento = DateTime.Today;
        public DateTime? FechaMovimiento
        {
            get => _fechaMovimiento;
            set { _fechaMovimiento = value; OnPropertyChanged(); }
        }

        private string _idEconomico = null;
        public string IdEconomico
        {
            get => _idEconomico;
            set { _idEconomico = value; OnPropertyChanged(); }
        }

        private string _anotaciones = null;
        public string Anotaciones
        {
            get => _anotaciones;
            set { _anotaciones = value; OnPropertyChanged(); }
        }

        private int _idUbicacionSalida;
        public int IdUbicacionSalida
        {
            get => _idUbicacionSalida;
            set { _idUbicacionSalida = value; OnPropertyChanged(); }
        }

        private int _idUbicacionLlegada;
        public int IdUbicacionLlegada
        {
            get => _idUbicacionLlegada;
            set { _idUbicacionLlegada = value; OnPropertyChanged(); }
        }

        private string _horaMovimiento = DateTime.Now.ToString("HH:mm");
        public string HoraMovimiento
        {
            get => _horaMovimiento;
            set { _horaMovimiento = value; OnPropertyChanged(); }
        }

        public ICommand AltaMovimientoCommand { get; }
        private ICollectionView _vistaHistorialMovimientos;
        public ICollectionView VistaHistorialMovimientos
        {
            get => _vistaHistorialMovimientos;
            set
            {
                _vistaHistorialMovimientos = value;
                OnPropertyChanged(nameof(VistaHistorialMovimientos));
            }
        }

        public ObservableCollection<CatalogoUbicacionesProyecto> Ubicaciones { get; set; }
        public ObservableCollection<EconomicoMinimoDto> Economicos { get; set; }
        public ObservableCollection<CatalogoMovimientosEconomico> MovimientosEconomicos { get; set; }
        public ObservableCollection<string> ArchivosSeleccionados { get; set; }


        private readonly UbicacionProyeectoService _ubicacionesService;
        private readonly GestorArchivosService _gestorArchivosService;
        private readonly CatalogoEconomicosService _economicosService;
        private readonly RealizarMovimientosService _movimientosService;

        public ICommand EliminarMovimientoCommand { get; }
        public ICommand SeleccionarArchivosCommand { get; }
        public ICommand AltaMovimietoCommand { get; }
        public ICommand VerArchivoCommand { get; }
        public ICommand EliminarArchivoCommand { get; }
        public ICommand RegistrarLlegadaCommand { get; }

        private string _filtroIdEconomico = string.Empty;
        public string FiltroIdEconomico
        {
            get => _filtroIdEconomico;
            set
            {
                _filtroIdEconomico = value;
                OnPropertyChanged();
                VistaHistorialMovimientos?.Refresh();
            }
        }

        public event EventHandler? MovimientoRegistrado;
        private readonly Action _onRegistroMovimientoCompletado;
        public RealizarMovimientoViewModel(
            UbicacionProyeectoService ubicacionService,
            CatalogoEconomicosService economicosService,
            RealizarMovimientosService realizarMovimientosService,
            GestorArchivosService gestorArchivosService,
            Action onRegistroMovimientoCompletado)
        {
            _ubicacionesService = ubicacionService;
            _economicosService = economicosService;
            _movimientosService = realizarMovimientosService;
            _gestorArchivosService = gestorArchivosService;

            SeleccionarArchivosCommand = new RelayCommand(SeleccionarArchivos);
            AltaMovimietoCommand = new RelayCommand(AltaMovimiento);
            VerArchivoCommand = new RelayCommand<CatalogoArchivo>(EjecutarAbrirArchivo);
            EliminarArchivoCommand = new RelayCommand<string>(EliminarArchivo);
            RegistrarLlegadaCommand = new RelayCommand<CatalogoMovimientosEconomico>(LlegadaEconomico);

            Economicos = new ObservableCollection<EconomicoMinimoDto>();
            MovimientosEconomicos = new ObservableCollection<CatalogoMovimientosEconomico>();
            Ubicaciones = new ObservableCollection<CatalogoUbicacionesProyecto>();
            ArchivosSeleccionados = new ObservableCollection<string>();

            _vistaHistorialMovimientos = CollectionViewSource.GetDefaultView(MovimientosEconomicos);
            _vistaHistorialMovimientos.Filter = FiltrarPorEconomico;

            _onRegistroMovimientoCompletado = onRegistroMovimientoCompletado;
            CargarTipos();
            CargarHistorialMovimientos();
        }

        public void LlegadaEconomico(CatalogoMovimientosEconomico? movimiento) 
        {
            if (movimiento == null) return;

            var editViewModel = new ReciboEquipoViewModel(movimiento, _movimientosService, _gestorArchivosService);
            var ventanaEditar = new LlegadaMovimientoView(editViewModel)
            {
                Owner = Application.Current.MainWindow
            };

            if (ventanaEditar.ShowDialog() == true)
            {
                CargarHistorialMovimientos();
            }
        }

        private void EliminarArchivo(string? ruta)
        {
            if (!string.IsNullOrEmpty(ruta) && ArchivosSeleccionados.Contains(ruta))
            {
                ArchivosSeleccionados.Remove(ruta);
            }
        }

        public void CargarTipos() 
        {
            var datoseconomicos = _economicosService.ObtenerEconomicosCortos();
            Economicos.Clear();
            foreach (var economico in datoseconomicos)
            {
                Economicos.Add(economico);
            }
            var datosUbicacion = _ubicacionesService.ObtenerUbicaciones();
            Ubicaciones.Clear();
            foreach (var ubicacion in datosUbicacion)
            {
                Ubicaciones.Add(ubicacion);
            }
        }

        private bool FiltrarPorEconomico(object obj)
        {

            if (obj is not CatalogoMovimientosEconomico movimiento) return false;
            if (string.IsNullOrWhiteSpace(FiltroIdEconomico)) return true;
            return movimiento.IdEconomico != null &&
                   movimiento.IdEconomico.Contains(FiltroIdEconomico, StringComparison.OrdinalIgnoreCase);
        }

        private void SeleccionarArchivos()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Todos los archivos (*.*)|*.*|Documentos PDF (*.pdf)|*.pdf|Imágenes (*.jpg;*.png)|*.jpg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string ruta in openFileDialog.FileNames)
                {
                    if (!ArchivosSeleccionados.Contains(ruta))
                    {
                        ArchivosSeleccionados.Add(ruta);
                    }
                }
            }
        }

        public void AltaMovimiento()
        {
            try
            {
                if (!FechaMovimiento.HasValue)
                {
                    MessageBox.Show("Debe seleccionar una fecha válida.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(IdEconomico))
                {
                    MessageBox.Show("Debe seleccionar un número económico.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (IdUbicacionLlegada == null)
                {
                    MessageBox.Show("Debe seleccionar un proyecto de llegada del equipo.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (IdUbicacionSalida == null)
                {
                    MessageBox.Show("Debe seleccionar un proyecto de salida del equipo.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                DateTime fechaBase = FechaMovimiento.Value.Date;
                if (TimeSpan.TryParse(HoraMovimiento, out TimeSpan horaParsed))
                {
                    fechaBase = fechaBase.Add(horaParsed);
                }
                
                var dto = new CatalogoMovimientosEconomico
                {
                    IdEconomico = this.IdEconomico,
                    IdUbicacionLlegada = this.IdUbicacionLlegada,
                    IdUbicacionSalida = this.IdUbicacionSalida,
                    FechaMovimiento = fechaBase,
                    Anotaciones = this.Anotaciones,
                    IdUsuario = App.Session.IdUsuario
                };

                CatalogoMovimientosEconomico movimientoCreado = _movimientosService.RegistrarMovimiento(App.Session.IdUsuario, dto);
                if (ArchivosSeleccionados.Count > 0)
                {
                    List<string> listaRutas = ArchivosSeleccionados.ToList();
                    var registrosProcesados = _gestorArchivosService.RegistrarArchivosMovimientos(listaRutas, movimientoCreado.IdMovimiento);
                    using (var contextoBD = new InventarioContext())
                    {
                        var logsService = new LogsService(contextoBD);
                        foreach (var item in registrosProcesados)
                        {
                            logsService.RegistrarDocumentoAdjuntoExitoso(App.Session.IdUsuario, item.Archivo.Archivo, movimientoCreado.IdMovimiento.ToString());
                        }
                    }
                }

                MessageBox.Show("Se registró el Servicio y sus archivos de manera exitosa.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                MovimientoRegistrado?.Invoke(this, EventArgs.Empty);
                _onRegistroMovimientoCompletado?.Invoke();

                CargarHistorialMovimientos();
            }
            catch (Exception ex)
            {
                string mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show(mensajeError, "Error de Base de Datos / FTP", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CargarHistorialMovimientos()
        {
            var datosmovimientos = _movimientosService.ObtenerHistorial();
            MovimientosEconomicos.Clear();

            foreach (var item in datosmovimientos)
            {
                MovimientosEconomicos.Add(item);
            }
            VistaHistorialMovimientos?.Refresh();
        }

        private void EjecutarAbrirArchivo(CatalogoArchivo archivo)
        {
            if (string.IsNullOrEmpty(archivo?.Archivo)) return;
            try
            {
                string rutaCompleta = _gestorArchivosService.ObtenerRutaAbsoluta(archivo.Archivo);

                if (!File.Exists(rutaCompleta))
                {
                    MessageBox.Show($"No se pudo recuperar el archivo físico desde el servidor de almacenamiento.",
                                    "Archivo No Encontrado", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = rutaCompleta,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el archivo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}