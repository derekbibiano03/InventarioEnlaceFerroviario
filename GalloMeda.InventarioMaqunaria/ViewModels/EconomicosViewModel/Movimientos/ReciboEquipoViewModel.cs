using Inventario.Core.Services.Economicos;
using Inventario.Data.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;


namespace Inventario.Desktop.ViewModels.EconomicosViewModel.Movimientos
{
    public class ReciboEquipoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event Action<bool>? RequestClose;

        private readonly RealizarMovimientosService _realizarMovimientosService;
        private readonly GestorArchivosService _gestorArchivosService;
        public int IdMovimiento { get; set; }

        private DateTime? _fechaLlegada;
        public DateTime? FechaLlegada
        {
            get => _fechaLlegada;
            set { _fechaLlegada = value; OnPropertyChanged(); }
        }

        private string _anotacionesLlegada;
        public string AnotacionesLlegada
        {
            get => _anotacionesLlegada;
            set { _anotacionesLlegada = value; OnPropertyChanged(); }
        }

        public ObservableCollection<MovimientosArchivo> ArchivosExistentes { get; set; }
        public ObservableCollection<string> NuevosArchivosSeleccionados { get; set; }
        private List<int> ArchivosAEliminarIds { get; set; }

        public ICommand GuardarCambiosCommand { get; }
        public ICommand SeleccionarArchivosCommand { get; }
        public ICommand QuitarNuevoArchivoCommand { get; }
        public ICommand EliminarArchivoExistenteCommand { get; }

        public ReciboEquipoViewModel(CatalogoMovimientosEconomico movimientoModel,
                                    RealizarMovimientosService servicioMovimiento,
                                    GestorArchivosService gestorArchivosService) 
        {
            _realizarMovimientosService = servicioMovimiento;
            _gestorArchivosService = gestorArchivosService;

            ArchivosExistentes = new ObservableCollection<MovimientosArchivo>();
            NuevosArchivosSeleccionados = new ObservableCollection<string>();
            ArchivosAEliminarIds = new List<int>();

            IdMovimiento = movimientoModel.IdMovimiento;
            FechaLlegada = movimientoModel.FechaLlegada;
            AnotacionesLlegada = movimientoModel.AnotacionesLlegada;

            if (movimientoModel.MovimientosArchivos != null)
            {
                foreach (var archivo in movimientoModel.MovimientosArchivos)
                {
                    ArchivosExistentes.Add(archivo);
                }
            }

            GuardarCambiosCommand = new RelayCommand(GuardarCambios);
            SeleccionarArchivosCommand = new RelayCommand(SeleccionarNuevosArchivos);
            QuitarNuevoArchivoCommand = new RelayCommand<string>(QuitarNuevoArchivo);
            EliminarArchivoExistenteCommand = new RelayCommand<MovimientosArchivo>(EliminarArchivoExistente);
        }

        private void EliminarArchivoExistente(MovimientosArchivo? archivo)
        {
            if (archivo != null && ArchivosExistentes.Contains(archivo))
            {
                ArchivosAEliminarIds.Add(archivo.IdArchivoNavigation.IdArchivo);
                ArchivosExistentes.Remove(archivo);
            }
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

        private void GuardarCambios()
        {
            try
            {
                if (!FechaLlegada.HasValue)
                {
                    MessageBox.Show("Debe seleccionar una fecha válida.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var movimientoActualizado = new CatalogoMovimientosEconomico
                {
                    IdMovimiento = this.IdMovimiento,
                    FechaLlegada = this.FechaLlegada,
                    AnotacionesLlegada = this.AnotacionesLlegada,
                };

                _realizarMovimientosService.ActualizarMovimiento(movimientoActualizado);

                if (ArchivosAEliminarIds.Count > 0)
                {
                    _gestorArchivosService.EliminarArchivosMovimientos(ArchivosAEliminarIds);
                }

                if (NuevosArchivosSeleccionados.Count > 0)
                {
                    _gestorArchivosService.RegistrarArchivosMovimientos(NuevosArchivosSeleccionados.ToList(), IdMovimiento);
                }

                MessageBox.Show("Servicio actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

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
