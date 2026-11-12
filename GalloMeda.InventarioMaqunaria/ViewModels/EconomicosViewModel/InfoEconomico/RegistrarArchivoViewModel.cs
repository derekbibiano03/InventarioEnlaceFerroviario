using GalloMeda.InventarioMaqunaria;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.EconomicosViewModel.InfoEconomico
{
    public class RegistrarArchivoViewModel : INotifyPropertyChanged
    {
        private readonly InventarioContext _contextoBD;
        private readonly LogsService _logsService;
        private readonly GestorArchivosService _gestorArchivosService;

        public ObservableCollection<CatalogoEconomico> ListaEconomicos { get; set; } = new ObservableCollection<CatalogoEconomico>();
        public ObservableCollection<string> ListaArchivosSeleccionados { get; set; } = new ObservableCollection<string>();

        public ICollectionView EconomicosFiltrados { get; set; }

        private string _textoBusquedaId = string.Empty;
        public string TextoBusquedaId
        {
            get => _textoBusquedaId;
            set
            {
                _textoBusquedaId = value;
                OnPropertyChanged();
                EconomicosFiltrados?.Refresh();
            }
        }

        private string _fechaAltaActual = DateTime.Now.ToString("dd/MM/yyyy");
        public string FechaAltaActual
        {
            get => _fechaAltaActual;
            set
            {
                _fechaAltaActual = value;
                OnPropertyChanged();
            }
        }

        public ICommand SeleccionarArchivosCommand { get; }
        public ICommand EliminarArchivoCommand { get; }
        public ICommand GuardarArchivoCommand { get; }
        public int UsuarioLog { get; private set; }

        public RegistrarArchivoViewModel()
        {
            _contextoBD = new InventarioContext();
            _gestorArchivosService = new GestorArchivosService(_contextoBD);
            _logsService = new LogsService(_contextoBD);

            ListaArchivosSeleccionados.CollectionChanged += (s, e) =>
            {
                (GuardarArchivoCommand as RelayCommand)?.RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(ListaArchivosSeleccionados));
            };

            SeleccionarArchivosCommand = new RelayCommand(EjecutarSeleccionarArchivos);
            EliminarArchivoCommand = new RelayCommand<string>(EjecutarEliminarArchivo);
            GuardarArchivoCommand = new RelayCommand(EjecutarGuardarArchivo, CanGuardarArchivo);

            CargarEconomicosDesdeBD();

            EconomicosFiltrados = CollectionViewSource.GetDefaultView(ListaEconomicos)!;
            EconomicosFiltrados.Filter = FiltroBusquedaId;
        }

        private void CargarEconomicosDesdeBD()
        {
            try
            {
                var listaBD = _contextoBD.CatalogoEconomicos.ToList();
                ListaEconomicos = new ObservableCollection<CatalogoEconomico>(listaBD);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar Económicos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ListaEconomicos = new ObservableCollection<CatalogoEconomico>();
            }
        }

        private bool FiltroBusquedaId(object? obj)
        {
            if (string.IsNullOrEmpty(TextoBusquedaId)) return true;

            if (obj is CatalogoEconomico economico && economico.IdEconomico != null)
            {
                return economico.IdEconomico.IndexOf(TextoBusquedaId, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return false;
        }

        private void EjecutarSeleccionarArchivos()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos permitidos (*.pdf;*.png;*.jpg;*.tif;*.JPEG)|*.pdf;*.png;*.jpg;*.tif;*.JPEG",
                Title = "Seleccione los documentos o imágenes",
                Multiselect = true // Permite seleccionar varios archivos a la vez
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string ruta in openFileDialog.FileNames)
                {
                    if (!ListaArchivosSeleccionados.Contains(ruta))
                    {
                        ListaArchivosSeleccionados.Add(ruta);
                    }
                }
            }
        }

        private void EjecutarEliminarArchivo(string? ruta)
        {
            if (!string.IsNullOrEmpty(ruta) && ListaArchivosSeleccionados.Contains(ruta))
            {
                ListaArchivosSeleccionados.Remove(ruta);
            }
        }

        private void EjecutarGuardarArchivo()
        {
            List<string> idsSeleccionados = ListaEconomicos
                .Where(x => x.IsSelected)
                .Select(x => x.IdEconomico)
                .Where(id => id != null)
                .Select(id => id!)
                .ToList();

            if (!idsSeleccionados.Any())
            {
                MessageBox.Show("Debes seleccionar al menos un económico.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ListaArchivosSeleccionados.Any())
            {
                MessageBox.Show("Debes agregar al menos un archivo.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Registra los archivos en FTP, crea CatalogoArchivos y genera EconomicosArchivos para todos los económicos seleccionados
                var registrosProcesados = _gestorArchivosService.RegistrarArchivosEconomicos(
                    ListaArchivosSeleccionados.ToList(),
                    idsSeleccionados
                );

                _contextoBD.SaveChanges();

                UsuarioLog = App.Session.IdUsuario;
                foreach (var item in registrosProcesados)
                {
                    _logsService.RegistrarDocumentoAdjuntoExitoso(UsuarioLog, item.Archivo.Archivo, item.IdEconomico);
                }

                MessageBox.Show($"Se asociaron {ListaArchivosSeleccionados.Count} archivo(s) a {idsSeleccionados.Count} económico(s) correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                LimpiarFormulario();
            }
            catch (DbUpdateException ex)
            {
                string mensajeBD = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Error de Base de Datos al insertar registros:\n{mensajeBD}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                string mensajeReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Error crítico al procesar archivos:\n{mensajeReal}", "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanGuardarArchivo()
        {
            return ListaArchivosSeleccionados.Any() && ListaEconomicos != null && ListaEconomicos.Any(x => x.IsSelected);
        }

        private void LimpiarFormulario()
        {
            ListaArchivosSeleccionados.Clear();
            TextoBusquedaId = string.Empty;

            foreach (var economico in ListaEconomicos)
            {
                economico.IsSelected = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}