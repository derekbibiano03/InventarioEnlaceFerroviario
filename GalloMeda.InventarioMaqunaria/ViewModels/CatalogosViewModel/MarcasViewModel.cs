using Inventario.Core.Services.Catalogos;
using Inventario.Data;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.CatalogosViewModel
{
    public class MarcasViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly CatalogoMarcasService _marcasService;
        public ObservableCollection<CatalogoMarca> Marcas { get; set; }

        private string _nombreMarca = string.Empty;
        public string NombreMarca
        {
            get => _nombreMarca;
            set { _nombreMarca = value; OnPropertyChanged(); }
        }

        public ICommand RegistrarMarcaCommand { get; }

        public MarcasViewModel()
        {
            var context = new InventarioContext();
            _marcasService = new CatalogoMarcasService(context);
            Marcas = new ObservableCollection<CatalogoMarca>();
            RegistrarMarcaCommand = new RelayCommand(EjecutarRegistrarMarca);
            CargarMarcas();
        }

        public void CargarMarcas()
        {
            Marcas.Clear();
            var datos = _marcasService.ObtenerMarcas();
            foreach (var dato in datos)
            {
                Marcas.Add(dato);
            }
        }

        private void EjecutarRegistrarMarca()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.NombreMarca))
                {
                    throw new Exception("El campo 'Nombre de Marca' es obligatorio.");
                }
                bool guardarRegistro = _marcasService.NuevaMarca(this.NombreMarca.Trim());

                if (guardarRegistro)
                {
                    MessageBox.Show("¡Marca registrada exitosamente!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    LimpiarFormulario();
                    CargarMarcas();
                }
            }
            catch (Exception ex)
            {
                string mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show(mensajeError, "Error al Registrar", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LimpiarFormulario()
        {
            this.NombreMarca = string.Empty;
        }
    }
}