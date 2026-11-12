using Inventario.Core.Services.Logs;
using Inventario.Core.Services.Personal;
using Inventario.Data;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.EconomicosViewModel.InfoEconomico
{
    public class EditarEconomicoViewModel : INotifyPropertyChanged
    {
        // Delegado para cerrar la ventana desde el ViewModel
        public System.Action<bool>? CloseAction { get; set; }

        // Contexto de base de datos
        private readonly InventarioContext _contexto;
        private readonly EmpleadoService _empleadosService;

        // Colecciones observables para los ComboBox
        public ObservableCollection<Empleado> Encargados { get; set; }
        public ObservableCollection<Empleado> Operadores { get; set; }
        public ObservableCollection<CatalogoTiposEquipo> TipoEquipo { get; set; }
        public ObservableCollection<CatalogoGrupo> Grupos { get; set; }
        public ObservableCollection<CatalogoMarca> Marcas { get; set; }
        public ObservableCollection<CatalogoEstatus> Estatus { get; set; }
        public ObservableCollection<Empleado> Empleados { get; set; }
        public ObservableCollection<CatalogoTiposCombustible> Combustibles { get; set; }
        public ObservableCollection<CatalogoPya> PYA { get; set; }
        public ObservableCollection<CatalogoUbicacionesProyecto> Ubicaciones { get; set; }

        // Entidad principal
        private CatalogoEconomico? _economicoEdicion;
        public CatalogoEconomico? EconomicoEdicion
        {
            get => _economicoEdicion;
            set
            {
                _economicoEdicion = value;
                OnPropertyChanged();
            }
        }

        // Propiedad wrapper bindada directamente al ComboBox en XAML
        private CatalogoGrupo? _grupoSeleccionado;
        public CatalogoGrupo? GrupoSeleccionado
        {
            get => _grupoSeleccionado;
            set
            {
                _grupoSeleccionado = value;
                if (EconomicoEdicion != null)
                {
                    // Asigna la clave foránea a la entidad principal
                    EconomicoEdicion.IdGrupo = value?.IdGrupo;
                }
                OnPropertyChanged();
            }
        }

        // Comandos
        public ICommand GuardarCambiosCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }

        // Constructor
        public EditarEconomicoViewModel()
        {
            _contexto = new InventarioContext();
            _empleadosService = new EmpleadoService(_contexto);

            Encargados = new ObservableCollection<Empleado>();
            Operadores = new ObservableCollection<Empleado>();
            TipoEquipo = new ObservableCollection<CatalogoTiposEquipo>();
            Grupos = new ObservableCollection<CatalogoGrupo>();
            Marcas = new ObservableCollection<CatalogoMarca>();
            Estatus = new ObservableCollection<CatalogoEstatus>();
            Combustibles = new ObservableCollection<CatalogoTiposCombustible>();
            PYA = new ObservableCollection<CatalogoPya>();
            Ubicaciones = new ObservableCollection<CatalogoUbicacionesProyecto>();

            GuardarCambiosCommand = new RelayCommand(EjecutarGuardarCambios);
            CancelarCommand = new RelayCommand(EjecutarCancelar);

            CargarCatalogos();
        }

        private void CargarCatalogos()
        {
            var tiposDb = _contexto.CatalogoTiposEquipos.AsNoTracking().ToList();
            foreach (var item in tiposDb)
            {
                item.IdTipoEquipo = item.IdTipoEquipo.Trim();
                TipoEquipo.Add(item);
            }

            var gruposDb = _contexto.CatalogoGrupos.AsNoTracking().ToList();
            foreach (var item in gruposDb)
            {
                item.IdGrupo = item.IdGrupo.Trim();
                Grupos.Add(item);
            }

            var marcasDb = _contexto.CatalogoMarcas.AsNoTracking().OrderBy(x => x.NombreMarca).ToList();
            foreach (var item in marcasDb) Marcas.Add(item);

            var estatusDb = _contexto.CatalogoEstatuses.AsNoTracking().OrderBy(x => x.DescripcionEstatus).ToList();
            foreach (var item in estatusDb) Estatus.Add(item);

            var combustiblesDb = _contexto.CatalogoTiposCombustibles.AsNoTracking().ToList();
            foreach (var item in combustiblesDb) Combustibles.Add(item);

            var pyaDb = _contexto.CatalogoPyas.AsNoTracking().ToList();
            foreach (var item in pyaDb) PYA.Add(item);

            var ubicacionesDb = _contexto.CatalogoUbicacionesProyectos.AsNoTracking().OrderBy(x => x.Ubicacion).ToList();
            foreach (var item in ubicacionesDb) Ubicaciones.Add(item);

            var operaPermitidos = new List<int> { 2, 3, 4, 5, 6, 7 };
            var operaDb = _empleadosService.ObtenerResponsables(operaPermitidos);
            foreach (var item in operaDb) Operadores.Add(item);

            var rolesPermitidos = new List<int> { 2, 3, 4, 5, 6, 7 };
            var respDb = _empleadosService.ObtenerResponsables(rolesPermitidos);
            foreach (var item in respDb) Encargados.Add(item);
        }

        public void CargarDatosEconomico(string idEconomico)
        {
            var economico = _contexto.CatalogoEconomicos
                .FirstOrDefault(e => e.IdEconomico == idEconomico);

            if (economico != null)
            {
                // Limpieza de cadenas
                if (!string.IsNullOrEmpty(economico.IdGrupo))
                {
                    economico.IdGrupo = economico.IdGrupo.Trim();
                }

                if (!string.IsNullOrEmpty(economico.IdTipoEquipo))
                {
                    economico.IdTipoEquipo = economico.IdTipoEquipo.Trim();
                }

                EconomicoEdicion = economico;

                if (!string.IsNullOrEmpty(economico.IdGrupo))
                {
                    // 1. Intenta buscar primero por clave/IdGrupo exacto
                    var grupoEncontrado = Grupos.FirstOrDefault(g => g.IdGrupo.Equals(economico.IdGrupo, StringComparison.OrdinalIgnoreCase));

                    // 2. Si no lo encuentra por clave (como pasa con 'adb'), busca por coincidencia en DescripcionGrupo
                    if (grupoEncontrado == null)
                    {
                        grupoEncontrado = Grupos.FirstOrDefault(g => g.DescripcionGrupo.Contains(economico.IdGrupo, StringComparison.OrdinalIgnoreCase));
                    }

                    // Asigna el grupo encontrado en el catálogo para seleccionar el elemento en la UI
                    if (grupoEncontrado != null)
                    {
                        GrupoSeleccionado = grupoEncontrado;
                    }
                }
            }
        }

        private void EjecutarGuardarCambios()
        {
            if (EconomicoEdicion == null) return;

            try
            {
                var logsService = new LogsService(_contexto);
                _contexto.SaveChanges();
                logsService.RegistrarModificacionEquipo(GalloMeda.InventarioMaqunaria.App.Session.IdUsuario, EconomicoEdicion.IdEconomico);
                CloseAction?.Invoke(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Cancela la edición
        private void EjecutarCancelar()
        {
            CloseAction?.Invoke(false);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}