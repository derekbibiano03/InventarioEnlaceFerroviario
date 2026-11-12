using GalloMeda.InventarioMaqunaria;
using Inventario.Core.DTOs;
using Inventario.Core.Services.Catalogos;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.Personal;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Data.Models;
using Inventario.Desktop.Views.UserControllers.Economicos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.EconomicosViewModel.InfoEconomico
{
    public class EconomicosAltaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand AltaEconomicoCommand { get; }

        private readonly Action _onRegistroCompletado;

        private readonly CatalogoMarcasService _marcasService;
        private readonly CatalogoTiposEquipoService _tipoEquipoService;
        private readonly CatalogoGruposService _gruposService;
        private readonly CatalogoCombustiblesService _combustiblesService;
        private readonly ProAdminService _pyaService;
        private readonly UbicacionProyeectoService _ubicacionService;
        private readonly EmpleadoService _empleadosService;
        private readonly CatalogoEconomicosService _economicosService;

        public ObservableCollection<CatalogoMarca> Marcas { get; set; }
        public ObservableCollection<CatalogoTiposEquipo> TipoEquipo { get; set; }
        public ObservableCollection<CatalogoGrupo> Grupos { get; set; }
        public ObservableCollection<CatalogoTiposCombustible> Combustibles { get; set; }
        public ObservableCollection<CatalogoPya> PYA { get; set; }
        public ObservableCollection<CatalogoUbicacionesProyecto> Ubicaciones { get; set; }
        public ObservableCollection<Empleado> Empleados { get; set; }
        public ObservableCollection<Empleado> Responsables { get; set; }
        public ObservableCollection<Empleado> Operadores { get; set; }

        private string _idTipoEquipoSeleccionado = "F";
        public string IdTipoEquipoSeleccionado { get => _idTipoEquipoSeleccionado; set { _idTipoEquipoSeleccionado = value; OnPropertyChanged(); } }

        private string _idGrupoSeleccionado = "xxx";
        public string IdGrupoSeleccionado { get => _idGrupoSeleccionado; set { _idGrupoSeleccionado = value; OnPropertyChanged(); } }

        private int _idCombustibleSeleccionado = 7;
        public int IdCombustibleSeleccionado { get => _idCombustibleSeleccionado; set { _idCombustibleSeleccionado = value; OnPropertyChanged(); } }

        private int _idMarcaSeleccionado = 12;
        public int IdMarcaSeleccionado { get => _idMarcaSeleccionado; set { _idMarcaSeleccionado = value; OnPropertyChanged(); } }

        private int _marcaMotorSeleccionado = 12;
        public int MarcaMotorSeleccionado { get => _marcaMotorSeleccionado; set { _marcaMotorSeleccionado = value; OnPropertyChanged(); } }

        private int _idPropietarioSeleccionado = 5;
        public int IdPropietarioSeleccionado { get => _idPropietarioSeleccionado; set { _idPropietarioSeleccionado = value; OnPropertyChanged(); } }

        private int _idAdministradorSeleccionado = 5;
        public int IdAdministradorSeleccionado { get => _idAdministradorSeleccionado; set { _idAdministradorSeleccionado = value; OnPropertyChanged(); } }

        private int _idUbicacionSeleccionado = 12;
        public int IdUbicacionSeleccionado { get => _idUbicacionSeleccionado; set { _idUbicacionSeleccionado = value; OnPropertyChanged(); } }

        private int _idOperadorSeleccionado = 1;
        public int IdOperadorSeleccionado { get => _idOperadorSeleccionado; set { _idOperadorSeleccionado = value; OnPropertyChanged(); } }

        private int _idResponsableSeleccionado = 12;
        public int IdResponsableSeleccionado { get => _idResponsableSeleccionado; set { _idResponsableSeleccionado = value; OnPropertyChanged(); } }

        private string _gradoPropiedadSeleccionado = "SIN IDENTIFICAR";
        public string GradoPropiedadSeleccionado { get => _gradoPropiedadSeleccionado; set { _gradoPropiedadSeleccionado = value; OnPropertyChanged(); } }

        private string _observaciones = "SIN OBSERVACIONES";
        public string Observaciones { get => _observaciones; set { _observaciones = value; OnPropertyChanged(); } }

        private string _modelo = "MODELO NO IDENTIFICADO";
        public string Modelo { get => _modelo; set { _modelo = value; OnPropertyChanged(); } }

        private string _serie = "SERIE SIN IDENTIFICAR";
        public string Serie { get => _serie; set { _serie = value; OnPropertyChanged(); } }

        private int? _periodoFab = 0;
        public int? PeriodoFab { get => _periodoFab; set { _periodoFab = value; OnPropertyChanged(); } }

        private int? _horometro = 0;
        public int? Horometro { get => _horometro; set { _horometro = value; OnPropertyChanged(); } }

        private string _modeloMotor = "MODELO DE MOTOR SIN IDENTIFICAR O NO TIENE MOTOR";
        public string ModeloMotor { get => _modeloMotor; set { _modeloMotor = value; OnPropertyChanged(); } }

        private string _serieMotor = "SERIE DE MOTOR SIN IDENTIFICAR O NO TIENE MOTOR";
        public string SerieMotor { get => _serieMotor; set { _serieMotor = value; OnPropertyChanged(); } }

        private string _familiaMotor = "FAMILIA DE MOTOR SIN IDENTIFICAR O NO TIENE MOTOR";
        public string FamiliaMotor { get => _familiaMotor; set { _familiaMotor = value; OnPropertyChanged(); } }

        private string _placas = "PLACAS SIN IDENTIFICAR O NO TIENE PLACAS";
        public string Placas { get => _placas; set { _placas = value; OnPropertyChanged(); } }

        private string _polizaAdj = "SIN POLIZA VER EN DOCUMENTOS ADJUNTOS";
        public string PolizaAdj { get => _polizaAdj; set { _polizaAdj = value; OnPropertyChanged(); } }

        private string _dimensiones = "DIMENSIONES AUN SIN MEDIR";
        public string Dimensiones { get => _dimensiones; set { _dimensiones = value; OnPropertyChanged(); } }

        private string _thk = "SIN IDENTIFICAR HOROMETRO O KILOMETRAJE";
        public string THK { get => _thk; set { _thk = value; OnPropertyChanged(); } }

        private string _tipoSeguro = "SIN SEGURO";
        public string TipoSeguro { get => _tipoSeguro; set { _tipoSeguro = value; OnPropertyChanged(); } }

        private bool _estatusSeguro = false;
        public bool EstatusSeguro { get => _estatusSeguro; set { _estatusSeguro = value; OnPropertyChanged(); } }

        public EconomicosAltaViewModel(CatalogoMarcasService marcasService,
                                       CatalogoTiposEquipoService tipoEquipoService,
                                       CatalogoGruposService grupoService,
                                       CatalogoCombustiblesService combustibleService,
                                       ProAdminService pyaService,
                                       UbicacionProyeectoService ubicacionService,
                                       EmpleadoService empleadoService,
                                       CatalogoEconomicosService economicosService,
                                       Action onRegistroCompletado)
        {
            _marcasService = marcasService;
            Marcas = new ObservableCollection<CatalogoMarca>();

            _tipoEquipoService = tipoEquipoService;
            TipoEquipo = new ObservableCollection<CatalogoTiposEquipo>();

            _gruposService = grupoService;
            Grupos = new ObservableCollection<CatalogoGrupo>();

            _combustiblesService = combustibleService;
            Combustibles = new ObservableCollection<CatalogoTiposCombustible>();

            _pyaService = pyaService;
            PYA = new ObservableCollection<CatalogoPya>();

            _ubicacionService = ubicacionService;
            Ubicaciones = new ObservableCollection<CatalogoUbicacionesProyecto>();

            _empleadosService = empleadoService;
            Empleados = new ObservableCollection<Empleado>();

            Responsables = new ObservableCollection<Empleado>();
            Operadores = new ObservableCollection<Empleado>();

            AltaEconomicoCommand = new RelayCommand(EjecutarAltaEconomico);
            _economicosService = economicosService;
            _onRegistroCompletado = onRegistroCompletado;

            CargarTipos();
        }
        
        private void CargarTipos()
        {
            var datosmarcas = _marcasService.ObtenerMarcas();
            foreach (var marca in datosmarcas) { Marcas.Add(marca); }

            var datos = _tipoEquipoService.ObtenerTiposEq();
            foreach (var item in datos) { TipoEquipo.Add(item); }

            var datoGrupo = _gruposService.ObtenerGrupos();
            foreach (var grupo in datoGrupo) { Grupos.Add(grupo); }

            var datoCombus = _combustiblesService.ObtenerCombustibles();
            foreach (var combus in datoCombus) { Combustibles.Add(combus); }

            var datospya = _pyaService.ObtenerPYA();
            foreach (var datopya in datospya) { PYA.Add(datopya); }

            var datosUbi = _ubicacionService.ObtenerUbicaciones();
            foreach (var datoU in datosUbi) { Ubicaciones.Add(datoU); }

            var rolesPermitidos = new List<int> { 2, 3, 4, 5, 6, 7 };
            var datosRes = _empleadosService.ObtenerResponsables(rolesPermitidos);
            foreach (var responsable in datosRes) { Responsables.Add(responsable); }

            var datosOpera = _empleadosService.ObtenerResponsables(rolesPermitidos);
            foreach (var operador in datosOpera) { Operadores.Add(operador); }
        }
        
        public event EventHandler? EconomicoRegistrado;
        private void EjecutarAltaEconomico()
        {
            try
            {
                var dto = new EconomicoAltaDto
                {
                    IdTipoEquipo = this.IdTipoEquipoSeleccionado,
                    IdGrupo = this.IdGrupoSeleccionado,
                    IdCombustible = this.IdCombustibleSeleccionado,
                    IdPropietario = this.IdPropietarioSeleccionado,
                    IdAdministrador = this.IdAdministradorSeleccionado,
                    IdUbicacion = this.IdUbicacionSeleccionado,
                    IdOperador = this.IdOperadorSeleccionado,
                    IdResponsable = this.IdResponsableSeleccionado,
                    GradoPropiedad = this.GradoPropiedadSeleccionado,
                    IdMarca = this.IdMarcaSeleccionado,
                    Modelo = this.Modelo,
                    Serie = this.Serie,
                    PeriodoFab = this.PeriodoFab ?? 0,
                    Placas = this.Placas,
                    MarcaMotor = this.MarcaMotorSeleccionado,
                    ModeloMotor = this.ModeloMotor,
                    SerieMotor = this.SerieMotor,
                    FamiliaMotor = this.FamiliaMotor,
                    Observaciones = this.Observaciones,
                    PolizaAdj = this.PolizaAdj,
                    EstatusSeguro = this.EstatusSeguro,
                    Horometro = this.Horometro ?? 0,
                    Dimensiones = this.Dimensiones,
                    THK = this.THK,
                    TipoSeguro = this.TipoSeguro,
                };

                _economicosService.RegistrarEconomico(App.Session.IdUsuario, dto);
                MessageBox.Show("¡Guardado exitosamente en PostgreSQL!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                EconomicoRegistrado?.Invoke(this, EventArgs.Empty);
                _onRegistroCompletado?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}