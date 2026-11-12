using GalloMeda.InventarioMaqunaria;
using Inventario.Core.Services.Economicos;
using Inventario.Data.Models;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.EconomicosViewModel.Servicios
{
    public class EditarServicioViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Evento delegante para notificar el cierre a la ventana
        public event Action<bool>? RequestClose;

        private readonly HistorialServicioService _historialServicioService;
        private readonly GestorArchivosService _gestorArchivosService;

        public int IdServicio { get; set; }

        private string _noEconomico;
        public string NoEconomico
        {
            get => _noEconomico;
            set { _noEconomico = value; OnPropertyChanged(); }
        }

        private DateTime? _fechaMantenimiento;
        public DateTime? FechaMantenimiento
        {
            get => _fechaMantenimiento;
            set { _fechaMantenimiento = value; OnPropertyChanged(); }
        }

        private string _tipoMantenimiento;
        public string TipoMantenimiento
        {
            get => _tipoMantenimiento;
            set { _tipoMantenimiento = value; OnPropertyChanged(); }
        }

        private string _anotaciones;
        public string Anotaciones
        {
            get => _anotaciones;
            set { _anotaciones = value; OnPropertyChanged(); }
        }

        private string _horaskilometrosreales;
        public string Horaskilometrosreales
        {
            get => _horaskilometrosreales;
            set { _horaskilometrosreales = value; OnPropertyChanged(); }
        }

        // Listas de trabajo
        public ObservableCollection<ServicioArchivo> ArchivosExistentes { get; set; }
        public ObservableCollection<string> NuevosArchivosSeleccionados { get; set; }
        private List<int> ArchivosAEliminarIds { get; set; }

        // Comandos
        public ICommand GuardarCambiosCommand { get; }
        public ICommand SeleccionarArchivosCommand { get; }
        public ICommand QuitarNuevoArchivoCommand { get; }
        public ICommand EliminarArchivoExistenteCommand { get; }

        public EditarServicioViewModel(HistorialServicio servicioEditar,
                                      HistorialServicioService historialServicioService,
                                      GestorArchivosService gestorArchivosService)
        {
            _historialServicioService = historialServicioService;
            _gestorArchivosService = gestorArchivosService;

            // Inicialización de colecciones
            ArchivosExistentes = new ObservableCollection<ServicioArchivo>();
            NuevosArchivosSeleccionados = new ObservableCollection<string>();
            ArchivosAEliminarIds = new List<int>();

            // Cargar datos actuales de la entidad
            IdServicio = servicioEditar.IdServicio;
            NoEconomico = servicioEditar.NoEconomico;
            FechaMantenimiento = servicioEditar.FechaMantenimiento.ToDateTime(TimeOnly.MinValue);
            TipoMantenimiento = servicioEditar.TipoMantenimiento;
            Anotaciones = servicioEditar.Anotaciones;
            Horaskilometrosreales = servicioEditar.Horaskilometrosreales;

            // Cargar archivos ligados actualmente
            if (servicioEditar.ServicioArchivos != null)
            {
                foreach (var archivo in servicioEditar.ServicioArchivos)
                {
                    ArchivosExistentes.Add(archivo);
                }
            }

            // Mapeo de comandos
            GuardarCambiosCommand = new RelayCommand(GuardarCambios);
            SeleccionarArchivosCommand = new RelayCommand(SeleccionarNuevosArchivos);
            QuitarNuevoArchivoCommand = new RelayCommand<string>(QuitarNuevoArchivo);
            EliminarArchivoExistenteCommand = new RelayCommand<ServicioArchivo>(EliminarArchivoExistente);
        }

        private void SeleccionarNuevosArchivos()
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
                    if (!NuevosArchivosSeleccionados.Contains(ruta))
                    {
                        NuevosArchivosSeleccionados.Add(ruta);
                    }
                }
            }
        }

        private void QuitarNuevoArchivo(string? ruta)
        {
            if (!string.IsNullOrEmpty(ruta) && NuevosArchivosSeleccionados.Contains(ruta))
            {
                NuevosArchivosSeleccionados.Remove(ruta);
            }
        }

        private void EliminarArchivoExistente(ServicioArchivo? archivo)
        {
            if (archivo != null && ArchivosExistentes.Contains(archivo))
            {
                // Guarda el ID del archivo para eliminar el registro de la BD al guardar
                ArchivosAEliminarIds.Add(archivo.IdArchivoNavigation.IdArchivo);
                // Remueve de la vista previa
                ArchivosExistentes.Remove(archivo);
            }
        }

        private void GuardarCambios()
        {
            try
            {
                if (!FechaMantenimiento.HasValue)
                {
                    MessageBox.Show("Debe seleccionar una fecha válida.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Instancia el objeto actualizado
                var servicioActualizado = new HistorialServicio
                {
                    IdServicio = this.IdServicio,
                    NoEconomico = this.NoEconomico,
                    FechaMantenimiento = DateOnly.FromDateTime(this.FechaMantenimiento.Value),
                    TipoMantenimiento = this.TipoMantenimiento,
                    Anotaciones = this.Anotaciones,
                    Horaskilometrosreales = this.Horaskilometrosreales
                };

                // 1. Actualizar el registro base del servicio en la BD
                _historialServicioService.ActualizarServicio(servicioActualizado);

                // 2. Eliminar de la base de datos/almacenamiento los archivos desvinculados
                if (ArchivosAEliminarIds.Count > 0)
                {
                    _gestorArchivosService.EliminarArchivosServicio(ArchivosAEliminarIds);
                }

                // 3. Subir y registrar los archivos nuevos
                if (NuevosArchivosSeleccionados.Count > 0)
                {
                    _gestorArchivosService.RegistrarArchivosServicios(NuevosArchivosSeleccionados.ToList(), IdServicio);
                }

                MessageBox.Show("Servicio actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Notifica a la ventana para cerrar devolviendo OK (true)
                RequestClose?.Invoke(true);
            }
            catch (Exception ex)
            {
                string mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Error al actualizar el servicio: {mensajeError}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}