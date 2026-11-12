using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.UbicacionProyectosViewModel
{
    public class DetallesViewModel : INotifyPropertyChanged
    {

        private readonly InventarioContext _contextoBD;
        // 1. Declaración de dependencias del servicio de archivos y el contexto de base de datos.
        private readonly GestorArchivosService _archivosService;
        private CatalogoEconomico? _detalle;
        private ObservableCollection<CatalogoArchivo> _archivosAnexados;

        // Propiedad que almacena el objeto principal con los detalles del equipo
        public CatalogoEconomico? Detalle
        {
            get => _detalle;
            set
            {
                _detalle = value;
                OnPropertyChanged();
            }
        }

        // Colección enlazada a la interfaz de usuario para mostrar los nombres de archivos
        public ObservableCollection<CatalogoArchivo> ArchivosAnexados
        {
            get => _archivosAnexados;
            set
            {
                _archivosAnexados = value;
                OnPropertyChanged();
            }
        }

        // Comandos expuestos al XAML
        public ICommand AbrirArchivoCommand { get; }
        public ICommand DescargarArchivoCommand { get; }

        public DetallesViewModel(string idEconomico)
        {
            _contextoBD = new InventarioContext();
            // 2. Inicialización del servicio encargado de interactuar con el servidor de archivos Ubuntu.
            _archivosService = new GestorArchivosService(_contextoBD);
            _archivosAnexados = new ObservableCollection<CatalogoArchivo>();
            AbrirArchivoCommand = new RelayCommand<CatalogoArchivo>(EjecutarAbrirArchivo);
            DescargarArchivoCommand = new RelayCommand<CatalogoArchivo>(EjecutarDescargarArchivo);
            CargarDatosCompletos(idEconomico);
        }

        private void CargarDatosCompletos(string idEconomico)
        {
            using (var context = new Data.Models.InventarioContext())
            {
                var contexto = new InventarioContext();
                var logsService = new LogsService(contexto);
                var servicio = new CatalogoEconomicosService(context, logsService);
                Detalle = servicio.ObtenerDetalleCompleto(idEconomico);

                // Si el equipo contiene registros en su tabla intermedia, extraemos los archivos lógicos
                if (Detalle?.EconomicosArchivos != null)
                {
                    var listaArchivos = Detalle.EconomicosArchivos
                        .Where(ae => ae.IdArchivoNavigation != null)
                        .Select(ae => ae.IdArchivoNavigation)
                        .ToList();

                    foreach (var archivo in listaArchivos)
                    {
                        if (archivo != null)
                        {
                            // Agrega el objeto de archivo a la colección enlazada con la UI.
                            ArchivosAnexados.Add(archivo);
                        }
                    }
                }
            }
        }

        // Método ejecutado al dar clic sobre el archivo en la lista
        private void EjecutarAbrirArchivo(CatalogoArchivo archivo)
        {
            if (string.IsNullOrEmpty(archivo?.Archivo)) return;
            try
            {
                string rutaCompleta = _archivosService.ObtenerRutaAbsoluta(archivo.Archivo);

                // 5. Validación que confirma que el archivo ahora sí existe físicamente en el entorno local para ser abierto.
                if (!File.Exists(rutaCompleta))
                {
                    MessageBox.Show($"No se pudo recuperar el archivo físico desde el servidor de almacenamiento.",
                                    "Archivo No Encontrado", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 6. Ejecuta el proceso de inicio del sistema operativo para abrir el archivo con su visor predeterminado (ej: Adobe Reader o Navegador).
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = rutaCompleta,
                    UseShellExecute = true // Requerido en .NET Core / .NET 5+ para que use el visor predeterminado del sistema.
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el archivo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EjecutarDescargarArchivo(CatalogoArchivo archivo)
        {
            // Validación previa que confirma la presencia del nombre de archivo.
            if (string.IsNullOrEmpty(archivo?.Archivo)) return;

            // Bloque try-catch para capturar fallos durante la copia de archivos en el sistema.
            try
            {
                // Solicita la ruta absoluta del origen a través del servicio de archivos.
                string rutaCompletaOrigen = _archivosService.ObtenerRutaAbsoluta(archivo.Archivo);

                // Comprueba la existencia real del archivo origen antes de intentar copiarlo.
                if (!File.Exists(rutaCompletaOrigen))
                {
                    // Muestra un mensaje de advertencia si la fuente no está disponible.
                    MessageBox.Show($"No se pudo recuperar el archivo de origen desde el servidor.",
                                    "Error de Origen", MessageBoxButton.OK, MessageBoxImage.Error);
                    // Detiene la ejecución.
                    return;
                }

                // Instanciación del cuadro de diálogo para que el usuario elija dónde guardar el archivo.
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    // Establece el nombre por defecto utilizando la propiedad de la entidad.
                    FileName = archivo.NombreArchivo,
                    // Extrae la extensión del archivo para aplicarla por defecto.
                    DefaultExt = Path.GetExtension(archivo.NombreArchivo),
                    // Configura los filtros de extensión visibles en el desplegable del SaveFileDialog.
                    Filter = $"Archivos ({Path.GetExtension(archivo.NombreArchivo)})|*{Path.GetExtension(archivo.NombreArchivo)}|Todos los archivos (*.*)|*.*"
                };

                // Muestra la ventana modal de guardado y comprueba si el usuario hizo clic en "Aceptar/Guardar".
                if (saveFileDialog.ShowDialog() == true)
                {
                    // Copia el archivo desde el caché local/servidor hacia la ruta seleccionada sobrescribiendo si existe.

                    File.Copy(rutaCompletaOrigen, saveFileDialog.FileName!, true);
                    // Notifica al usuario de la correcta descarga y guardado.
                    MessageBox.Show("Archivo descargado y guardado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            // Captura de errores inesperados durante el diálogo o la copia física.
            catch (Exception ex)
            {
                // Muestra el mensaje detallado de la excepción ocurrida.
                MessageBox.Show($"Error al guardar el archivo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        // Método auxiliar protegido que dispara el evento PropertyChanged utilizando el nombre del miembro llamador.
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            // Dispara el evento si hay suscriptores (la vista WPF) escuchando los cambios.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}