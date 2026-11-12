using GalloMeda.InventarioMaqunaria;
using Inventario.Core.Services.Auth;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.Auth
{
    public class AgregarUsuarioViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand RegistrarUsuarioCommand { get; }

        private string _nombreUsuario = string.Empty;
        public string NombreUsuario
        {
            get => _nombreUsuario;
            set { _nombreUsuario = value; OnPropertyChanged(); }
        }

        private string _contrasenaPlana = string.Empty;
        public string ContrasenaPlana
        {
            get => _contrasenaPlana;
            set { _contrasenaPlana = value; OnPropertyChanged(); }
        }

        private int? _idRolSeleccionado;
        public int? IdRolSeleccionado
        {
            get => _idRolSeleccionado;
            set { _idRolSeleccionado = value; OnPropertyChanged(); }
        }

        private readonly AutenticacionService _autenticacionService;

        public ObservableCollection<UsuariosRole> Roles { get; set; }

        public AgregarUsuarioViewModel(AutenticacionService autenticacionService) {
            _autenticacionService = autenticacionService;
            Roles = new ObservableCollection<UsuariosRole>();
            RegistrarUsuarioCommand = new RelayCommand(EjecutarRegistrarUsuario);
            cargarinformacion();
        }

        public void cargarinformacion() {
            var datosrol = _autenticacionService.ObtenerRoles();
            foreach (var role in datosrol)
            {
                Roles.Add(role);
            }
        }

        private void EjecutarRegistrarUsuario()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.NombreUsuario))
                {
                    throw new Exception("El campo 'Nombre de Usuario' es obligatorio.");
                }

                if (string.IsNullOrWhiteSpace(this.ContrasenaPlana))
                {
                    throw new Exception("El campo 'Contraseña de Acceso' es obligatorio.");
                }

                if (this.ContrasenaPlana.Trim().Length < 6)
                {
                    throw new Exception("La contraseña debe contener al menos 6 caracteres.");
                }

                // CORRECCIÓN CRÍTICA: Validamos que el usuario haya seleccionado un rol del ComboBox
                if (this.IdRolSeleccionado == null || this.IdRolSeleccionado <= 0)
                {
                    throw new Exception("Debe seleccionar un 'Tipo de Usuario' válido.");
                }

                // CORRECCIÓN: Enviamos el IdRolSeleccionado convertido a int (usando .Value) hacia Postgres
                bool guardadoExitoso = _autenticacionService.RegistrarUsuario(
                    App.Session.IdUsuario,
                    this.NombreUsuario.Trim(),
                    this.ContrasenaPlana.Trim(),
                    this.IdRolSeleccionado.Value
                );

                if (guardadoExitoso)
                {
                    MessageBox.Show("¡Usuario registrado exitosamente con contraseña encriptada (BCrypt)!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
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
            this.NombreUsuario = string.Empty;
            this.ContrasenaPlana = string.Empty;
            this.IdRolSeleccionado = null;
        }
        
    }
}
