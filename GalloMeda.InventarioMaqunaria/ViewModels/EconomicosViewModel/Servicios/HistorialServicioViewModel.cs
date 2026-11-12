using GalloMeda.InventarioMaqunaria;
using Inventario.Core;
using Inventario.Core.DTOs;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Data.Models;
using Inventario.Desktop.Views;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.EconomicosViewModel.Servicios
{
    public class HistorialServicioViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand AltaServicioCommand { get; }
        public ICommand SeleccionarArchivosCommand { get; }
        public ICommand EliminarArchivoCommand { get; }
        public ICommand VerArchivoCommand { get; }
        public ICommand EliminarServicioCommand { get; }
        public ICommand AbrirEditarServicioCommand { get; }

        private ICollectionView _vistaHistorialServicios;
        public ICollectionView VistaHistorialServicios
        {
            get => _vistaHistorialServicios;
            set { _vistaHistorialServicios = value; OnPropertyChanged(); }
        }

        private readonly CatalogoEconomicosService _economicosService;
        private readonly HistorialServicioService _historialServicio;
        private readonly GestorArchivosService _gestorArchivosService;
        private readonly InventarioContext _context;
        private readonly UbicacionProyeectoService _ubicacionesService;

        public ObservableCollection<EconomicoMinimoDto> Economicos { get; set; }
        public ObservableCollection<HistorialServicio> HistorialServicio { get; set; }
        public ObservableCollection<CatalogoUbicacionesProyecto> Ubicaciones { get; set; }
        public ObservableCollection<string> ArchivosSeleccionados { get; set; }

        // PROPIEDAD PARA EL FILTRO DEL DATA GRID
        private string _filtroNoEconomico = string.Empty;
        public string FiltroNoEconomico
        {
            get => _filtroNoEconomico;
            set
            {
                _filtroNoEconomico = value;
                OnPropertyChanged();
                // Notifica a la vista que debe re-evaluar la condición del filtro
                VistaHistorialServicios?.Refresh();
            }
        }

        private string _noEconomico = null;
        public string NoEconomico
        {
            get => _noEconomico;
            set { _noEconomico = value; OnPropertyChanged(); }
        }

        private DateTime? _fechaMantenimiento = DateTime.Today;
        public DateTime? FechaMantenimiento
        {
            get => _fechaMantenimiento;
            set { _fechaMantenimiento = value; OnPropertyChanged(); }
        }

        private string _tipoMantenimiento = null;
        public string TipoMantenimiento
        {
            get => _tipoMantenimiento;
            set { _tipoMantenimiento = value; OnPropertyChanged(); }
        }
        

        private string _anotaciones = null;
        public string Anotaciones
        {
            get => _anotaciones;
            set { _anotaciones = value; OnPropertyChanged(); }
        }

        private string _horaskilometrosreales = null;
        public string Horaskilometrosreales
        {
            get => _horaskilometrosreales;
            set { _horaskilometrosreales = value; OnPropertyChanged(); }
        }

        private decimal _costos = 0;
        public decimal Costos
        {
            get => _costos;
            set { _costos = value; OnPropertyChanged(); }
        }


        public ICommand ExportarExcelCommand { get; }
        private readonly ExcelExportService _excelExportService;

        public event EventHandler? ServicioRegistrado;
        private readonly Action _onRegistroCompletado;
        public HistorialServicioViewModel(CatalogoEconomicosService economicosService,
                                          HistorialServicioService historialServiciosService,
                                          GestorArchivosService gestorArchivosService,
                                          UbicacionProyeectoService ubicacionesService,
                                          Action onRegistroCompletado)
        {
            _economicosService = economicosService;
            _historialServicio = historialServiciosService;
            _gestorArchivosService = gestorArchivosService;
            _ubicacionesService = ubicacionesService;
            _excelExportService = new ExcelExportService(_context);

            Economicos = new ObservableCollection<EconomicoMinimoDto>();
            HistorialServicio = new ObservableCollection<HistorialServicio>();
            Ubicaciones  = new ObservableCollection<CatalogoUbicacionesProyecto>();
            ArchivosSeleccionados = new ObservableCollection<string>();

            VistaHistorialServicios = CollectionViewSource.GetDefaultView(HistorialServicio);
            VistaHistorialServicios.Filter = FiltrarPorEconomico;

            AltaServicioCommand = new RelayCommand(AltaServicio);
            SeleccionarArchivosCommand = new RelayCommand(SeleccionarArchivos);
            EliminarArchivoCommand = new RelayCommand<string>(EliminarArchivo);
            EliminarServicioCommand = new RelayCommand<HistorialServicio>(EliminarServicio);
            VerArchivoCommand = new RelayCommand<CatalogoArchivo>(EjecutarAbrirArchivo);
            AbrirEditarServicioCommand = new RelayCommand<HistorialServicio>(AbrirEditarServicio);
            
            ExportarExcelCommand = new RelayCommand(ExportarExcel);
            _onRegistroCompletado = onRegistroCompletado;

            CargarTipos();
            CargarHistorialMovimientos();
        }

        // MÉTODO PREDICADO QUE FILTRA CADA REGISTRO DE LA LISTA
        private bool FiltrarPorEconomico(object obj)
        {
            
            if (obj is not HistorialServicio servicio) return false;
            if (string.IsNullOrWhiteSpace(FiltroNoEconomico)) return true;
            return servicio.NoEconomico != null &&
                   servicio.NoEconomico.Contains(FiltroNoEconomico, StringComparison.OrdinalIgnoreCase);
        }
        private void ExportarExcel()
        {
            try
            {
                // Obtenemos los elementos filtrados o toda la lista actual de la vista
                var listaAExportar = VistaHistorialServicios.Cast<HistorialServicio>().ToList();

                if (listaAExportar.Count == 0)
                {
                    MessageBox.Show("No hay registros para exportar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Archivos de Excel (*.xlsx)|*.xlsx",
                    FileName = $"Historial_Servicios_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    byte[] archivoBytes = _excelExportService.GenerarExcelHistorialServicios(listaAExportar);
                    File.WriteAllBytes(saveFileDialog.FileName, archivoBytes);

                    MessageBox.Show("El archivo de Excel se ha generado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar a Excel: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AbrirEditarServicio(HistorialServicio? servicio)
        {
            if (servicio == null) return;

            var editViewModel = new EditarServicioViewModel(servicio, _historialServicio, _gestorArchivosService);
            var ventanaEditar = new EditarServicioView(editViewModel)
            {
                Owner = Application.Current.MainWindow
            };

            if (ventanaEditar.ShowDialog() == true)
            {
                CargarHistorialMovimientos();
            }
        }
        private void EliminarServicio(HistorialServicio? servicio)
        {
            if (servicio == null) return;

            // Solicita confirmación al usuario antes de proceder
            var resultado = MessageBox.Show(
                $"¿Está seguro de eliminar el registro de servicio ID '{servicio.IdServicio}' del económico '{servicio.NoEconomico}'?\n\nEsta acción también eliminará sus archivos adjuntos asociados y no se puede deshacer.",
                "Confirmar Eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    // Llama al servicio para borrar de la base de datos
                    _historialServicio.EliminarServicio(servicio.IdServicio);

                    MessageBox.Show("El registro de servicio ha sido eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Refresca la tabla
                    CargarHistorialMovimientos();
                }
                catch (Exception ex)
                {
                    string mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    MessageBox.Show($"Error al intentar eliminar el servicio: {mensajeError}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void CargarHistorialMovimientos()
        {
            var datoshistorial = _historialServicio.ObtenerHistorial();
            HistorialServicio.Clear();

            foreach (var item in datoshistorial)
            {
                HistorialServicio.Add(item);
            }
            VistaHistorialServicios?.Refresh();
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

        private void EliminarArchivo(string? ruta)
        {
            if (!string.IsNullOrEmpty(ruta) && ArchivosSeleccionados.Contains(ruta))
            {
                ArchivosSeleccionados.Remove(ruta);
            }
        }

        public void AltaServicio()
        {
            try
            {
                // 1. Validaciones
                if (!FechaMantenimiento.HasValue)
                {
                    MessageBox.Show("Debe seleccionar una fecha válida.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NoEconomico))
                {
                    MessageBox.Show("Debe seleccionar un número económico.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(TipoMantenimiento))
                {
                    MessageBox.Show("Debe seleccionar un tipo de mantenimiento.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Horaskilometrosreales))
                {
                    MessageBox.Show("Debe agregar el horometro real del equipo al que se le realizo al equipo.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (Costos == null || Costos == 0)
                {
                    MessageBox.Show("Debes escribir el costo del servicio.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                // 2. Construcción del DTO
                var dto = new HistorialServicio
                {
                    NoEconomico = this.NoEconomico,
                    FechaMantenimiento = DateOnly.FromDateTime(this.FechaMantenimiento.Value),
                    TipoMantenimiento = this.TipoMantenimiento,
                    Anotaciones = this.Anotaciones,
                    Horaskilometrosreales = this.Horaskilometrosreales,
                    Costos = this.Costos,
                    IdUsuario = App.Session.IdUsuario
                };

                // 3. Registro en Base de Datos
                HistorialServicio servicioCreado = _historialServicio.RegistrarServicio(App.Session.IdUsuario, dto);

                // 4. Procesamiento de Archivos y Bitácora (Logs)
                if (ArchivosSeleccionados.Count > 0)
                {
                    List<string> listaRutas = ArchivosSeleccionados.ToList();

                    // Sube al FTP y genera registros en base de datos
                    var registrosProcesados = _gestorArchivosService.RegistrarArchivosServicios(listaRutas, servicioCreado.IdServicio);

                    // CORRECCIÓN: Instanciar LogsService para guardar la bitácora de los archivos subidos
                    using (var contextoBD = new InventarioContext())
                    {
                        var logsService = new LogsService(contextoBD);
                        foreach (var item in registrosProcesados)
                        {
                            logsService.RegistrarDocumentoAdjuntoExitoso(App.Session.IdUsuario, item.Archivo.Archivo, servicioCreado.IdServicio.ToString());
                        }
                    }
                }

                MessageBox.Show("Se registró el Servicio y sus archivos de manera exitosa.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                ServicioRegistrado?.Invoke(this, EventArgs.Empty);
                _onRegistroCompletado?.Invoke();

                CargarHistorialMovimientos();
            }
            catch (Exception ex)
            {
                string mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show(mensajeError, "Error de Base de Datos / FTP", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}