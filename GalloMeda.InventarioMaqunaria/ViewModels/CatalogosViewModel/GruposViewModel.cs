using Inventario.Core.Services.Catalogos;
using Inventario.Data;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.CatalogosViewModel
{
    public class GruposViewModel : INotifyPropertyChanged
    {
        private readonly CatalogoGruposService _gruposService;

        public ObservableCollection<CatalogoGrupo> Grupos { get; set; }

        public ICommand RegistrarGrupoCommand { get; }

        private string _descripcionCorta = string.Empty;
        public string DescripcionCorta
        {
            get => _descripcionCorta;
            set { _descripcionCorta = value; OnPropertyChanged(); }
        }

        private string _descripcionCompleta = string.Empty;
        public string DescripcionCompleta
        {
            get => _descripcionCompleta;
            set
            {
                _descripcionCompleta = value; OnPropertyChanged();
            }
        }

        public GruposViewModel()
        {
            var contexto = new InventarioContext();
            _gruposService = new CatalogoGruposService(contexto);
            Grupos = new ObservableCollection<CatalogoGrupo>();
            RegistrarGrupoCommand = new RelayCommand(EjecutarRegistrarGrupo);
            _ = CargarGruposAsync();
        }

        private void EjecutarRegistrarGrupo()
        {
            try
            {

                if (string.IsNullOrWhiteSpace(this.DescripcionCorta))
                {
                    throw new Exception("El campo 'DescripcionCorta' es obligatorio.");
                }
                if (string.IsNullOrWhiteSpace(this.DescripcionCompleta))
                {
                    throw new Exception("El campo 'DescripcionCompleta' es obligatorio.");
                }

                bool guardarRegistro = _gruposService.NuevoGrupo(
                    this.DescripcionCorta.Trim(),
                    this.DescripcionCompleta.Trim());

                if (guardarRegistro)
                {
                    MessageBox.Show("¡Grupo registrado exitosamente!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    LimpiarFormulario();
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
            this.DescripcionCompleta = string.Empty;
            this.DescripcionCorta = string.Empty;
        }

        public async Task CargarGruposAsync()
        {
            Grupos.Clear();

            var datos = await Task.Run(() => _gruposService.ObtenerGrupos());

            foreach (var dato in datos)
            {
                if (dato.DescripcionGrupo != null)
                {
                    dato.IdGrupo = dato.IdGrupo;
                    dato.DescripcionGrupo = dato.DescripcionGrupo.ToUpper();
                }
                Grupos.Add(dato);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}