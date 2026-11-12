using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using GalloMeda.InventarioMaqunaria;
using Inventario.Core.Services.Adq_Serv.AdquisicionService;
using Inventario.Core.Services.Adq_Serv.OrdenesCompra;
using Inventario.Core.Services.Auth;
using Inventario.Core.Services.Catalogos;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.AdqServ.OrdenesCompra
{
    public class OrdenCompraViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<CatalogoProveedore> Proveedores { get; } = new();

        private readonly InventarioContext _contexto;
        private readonly CatalogoProveedoresService _CatProveedoresService;
        private readonly string _idRequisicion; // Almacena el ID recibido

        private string? _idRequisicionTexto;
        public string? IdRequisicionTexto
        {
            get => _idRequisicionTexto;
            set { _idRequisicionTexto = value; OnPropertyChanged(); }
        }

        private CatalogoProveedore? _proveedorSeleccionado;
        public CatalogoProveedore? ProveedorSeleccionado
        {
            get => _proveedorSeleccionado;
            set
            {
                if (_proveedorSeleccionado != value)
                {
                    _proveedorSeleccionado = value;
                    OnPropertyChanged();
                    ActualizarDatosProveedor();
                }
            }
        }

        private string? _empresaSeleccionada;
        public string? EmpresaSeleccionada
        {
            get => _empresaSeleccionada;
            set
            {
                if (_empresaSeleccionada != value)
                {
                    _empresaSeleccionada = value;
                    OnPropertyChanged();
                    ActualizarDatosEmpresa();
                }
            }
        }

        private string? _razonSocial;
        public string? RazonSocial
        {
            get => _razonSocial;
            set { _razonSocial = value; OnPropertyChanged(); }
        }

        private string? _razonSocialEmpresa;
        public string? RazonSocialEmpresa
        {
            get => _razonSocialEmpresa;
            set { _razonSocialEmpresa = value; OnPropertyChanged(); }
        }

        private string? _telefono;
        public string? Telefono
        {
            get => _telefono;
            set { _telefono = value; OnPropertyChanged(); }
        }

        private string? _correoElectronico;
        public string? CorreoElectronico
        {
            get => _correoElectronico;
            set { _correoElectronico = value; OnPropertyChanged(); }
        }

        private string? _direccionCompleta;
        public string? DireccionCompleta
        {
            get => _direccionCompleta;
            set { _direccionCompleta = value; OnPropertyChanged(); }
        }

        private string? _direccionCompletaEmpresa;
        public string? DireccionCompletaEmpresa
        {
            get => _direccionCompletaEmpresa;
            set { _direccionCompletaEmpresa = value; OnPropertyChanged(); }
        }

        private string? _rfc;
        public string? Rfc
        {
            get => _rfc;
            set { _rfc = value; OnPropertyChanged(); }
        }

        private string? _regimenFiscal;
        public string? RegimenFiscal
        {
            get => _regimenFiscal;
            set { _regimenFiscal = value; OnPropertyChanged(); }
        }

        private string? _contacto;
        public string? Contacto
        {
            get => _contacto;
            set { _contacto = value; OnPropertyChanged(); }
        }

        private void ActualizarDatosProveedor()
        {
            if (_proveedorSeleccionado != null)
            {
                RazonSocial = _proveedorSeleccionado.RazonSocial ?? string.Empty;

                var partesDireccion = new[]
                {
                    _proveedorSeleccionado.Calle,
                    _proveedorSeleccionado.Numero,
                    _proveedorSeleccionado.Colonia,
                    _proveedorSeleccionado.Ciudad,
                    _proveedorSeleccionado.Estado
                };

                DireccionCompleta = string.Join(", ", partesDireccion.Where(p => !string.IsNullOrWhiteSpace(p)));
                Telefono = _proveedorSeleccionado.NumeroTelefonico ?? string.Empty;
                CorreoElectronico = _proveedorSeleccionado.CorreoElectronico ?? string.Empty;
                Contacto = _proveedorSeleccionado.Contacto ?? string.Empty;
            }
            else
            {
                RazonSocial = string.Empty;
                DireccionCompleta = string.Empty;
                Telefono = string.Empty;
                CorreoElectronico = string.Empty;
                Contacto = string.Empty;
            }
        }

        private void ActualizarDatosEmpresa()
        {
            if (_empresaSeleccionada == "CONSTRUCTORA GALLO MEDA")
            {
                RazonSocialEmpresa = "CONSTRUCTORA GALLO MEDA";
                DireccionCompletaEmpresa = "Detroit No. 16 Col. Ferrocarril, C.P. 44440, Guadalajara, Jal. Mex. Tel (33) 3942 3070";
                Rfc = "CGM81093083A";
                RegimenFiscal = "SOCIEDAD ANONIMA DE CAPITAL VARIABLE";
            }
            else if (_empresaSeleccionada == "OX TRANSPORTE Y LOGISTICA")
            {
                RazonSocialEmpresa = "OX TRANSPORTE Y LOGISTICA";
                DireccionCompletaEmpresa = "Detroit No. 16 Col. Ferrocarril, C.P. 44440, Guadalajara, Jal. Mex. Tel (33) 3942 3070";
                Rfc = "OTL200904QF9";
                RegimenFiscal = "SOCIEDAD ANONIMA DE CAPITAL VARIABLE";
            }
            else
            {
                RazonSocialEmpresa = string.Empty;
                DireccionCompletaEmpresa = string.Empty;
                Rfc = string.Empty;
                RegimenFiscal = string.Empty;
            }
        }

        // Constructor modificado para recibir el ID de la requisición
        public OrdenCompraViewModel(InventarioContext context, string idRequisicion)
        {
            _contexto = context ?? throw new ArgumentNullException(nameof(context));
            _idRequisicion = idRequisicion;
            IdRequisicionTexto = idRequisicion; // Puedes mostrarlo en un TextBlock de tu vista si gustas

            _CatProveedoresService = new CatalogoProveedoresService(context);
            CargarCatalogos();
        }

        public void CargarCatalogos()
        {
            Proveedores.Clear();
            var datosproveedores = _CatProveedoresService.ObtenerProveedores();
            if (datosproveedores != null)
            {
                foreach (var proveedor in datosproveedores)
                {
                    Proveedores.Add(proveedor);
                }
            }
        }
    }
}