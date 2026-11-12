using Inventario.Core.DTOs;
using Inventario.Core.DTOs.Materiales;
using Inventario.Core.Services.Catalogos;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.CatalogosViewModel
{
    public class CatalogoMaterialesViewModel : INotifyPropertyChanged
    {
        private bool _isResetting = false;

        public class OpcionFiltroCheckbox : INotifyPropertyChanged
        {
            private bool _isChecked;
            public string? Nombre { get; set; }
            public string? IdString { get; set; }

            public bool IsChecked
            {
                get => _isChecked;
                set
                {
                    if (_isChecked != value)
                    {
                        _isChecked = value;
                        OnPropertyChanged();
                        AlCambiarSeleccion?.Invoke();
                    }
                }
            }

            public required Action AlCambiarSeleccion { get; set; }
            public event PropertyChangedEventHandler? PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string? name = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        private string _busquedaDescripcion = string.Empty;
        private string _busquedaMarca = string.Empty;
        private string _busquedaNoParte = string.Empty;
        private string _busquedaClasificacion = string.Empty;
        private string _busquedaSubclasificacion = string.Empty;
        private string _busquedaGrupo = string.Empty;

        private readonly Dictionary<string, bool> _estadosDescripcion = new();
        private readonly Dictionary<string, bool> _estadosMarca = new();
        private readonly Dictionary<string, bool> _estadosNoParte = new();
        private readonly Dictionary<string, bool> _estadosClasificacion = new();
        private readonly Dictionary<string, bool> _estadosSubclasificacion = new();
        private readonly Dictionary<string, bool> _estadosGrupo = new();

        public ObservableCollection<OpcionFiltroCheckbox> FiltroDescripcionesOpciones { get; set; } = new();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroMarcasOpciones { get; set; } = new();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroNoParteOpciones { get; set; } = new();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroClasificacionesOpciones { get; set; } = new();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroSubclasificacionesOpciones { get; set; } = new();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroGrupoOpciones { get; set; } = new();

        public ICommand LimpiarFiltrosCommand { get; }

        public string BusquedaDescripcion
        {
            get => _busquedaDescripcion;
            set { _busquedaDescripcion = value; OnPropertyChanged(); FiltrarListaCheckboxes(FiltroDescripcionesOpciones, _busquedaDescripcion); }
        }

        public string BusquedaMarca
        {
            get => _busquedaMarca;
            set { _busquedaMarca = value; OnPropertyChanged(); FiltrarListaCheckboxes(FiltroMarcasOpciones, _busquedaMarca); }
        }

        public string BusquedaNoParte
        {
            get => _busquedaNoParte;
            set { _busquedaNoParte = value; OnPropertyChanged(); FiltrarListaCheckboxes(FiltroNoParteOpciones, _busquedaNoParte); }
        }

        public string BusquedaClasificacion
        {
            get => _busquedaClasificacion;
            set { _busquedaClasificacion = value; OnPropertyChanged(); FiltrarListaCheckboxes(FiltroClasificacionesOpciones, _busquedaClasificacion); }
        }

        public string BusquedaSubclasificacion
        {
            get => _busquedaSubclasificacion;
            set { _busquedaSubclasificacion = value; OnPropertyChanged(); FiltrarListaCheckboxes(FiltroSubclasificacionesOpciones, _busquedaSubclasificacion); }
        }

        public string BusquedaGrupo
        {
            get => _busquedaGrupo;
            set { _busquedaGrupo = value; OnPropertyChanged(); FiltrarListaCheckboxes(FiltroGrupoOpciones, _busquedaGrupo); }
        }

        private readonly CatalogoMateriales _catalogoMaterialesService;
        private readonly InventarioContext _context;

        public ObservableCollection<MaterialesDto> Materiales { get; set; } = new();
        public ICollectionView VistaMateriales { get; set; }

        public ObservableCollection<CatalogoGrupo> ListaGrupos { get; set; } = new();
        public ObservableCollection<CatalogoClasificacione> ListaClasificaciones { get; set; } = new();
        public ObservableCollection<CatalogoSubclasificacione> ListaSubclasificaciones { get; set; } = new();
        public ObservableCollection<CatalogoMarca> ListaMarcas { get; set; } = new();

        public CatalogoMaterialesViewModel()
        {
            _context = new InventarioContext();
            _catalogoMaterialesService = new CatalogoMateriales(_context);

            LimpiarFiltrosCommand = new RelayCommand(LimpiarFiltros);

            CargarDatosIniciales();

            VistaMateriales = CollectionViewSource.GetDefaultView(Materiales);
            VistaMateriales.Filter = FiltroEjecucion;

            RecalcularOpcionesFiltros();
        }

        public void CargarDatosIniciales()
        {
            ListaGrupos = new ObservableCollection<CatalogoGrupo>(_context.CatalogoGrupos.ToList());
            ListaClasificaciones = new ObservableCollection<CatalogoClasificacione>(_context.CatalogoClasificaciones.ToList());
            ListaSubclasificaciones = new ObservableCollection<CatalogoSubclasificacione>(_context.CatalogoSubclasificaciones.ToList());
            ListaMarcas = new ObservableCollection<CatalogoMarca>(_context.CatalogoMarcas.ToList());

            var datosmateriales = _catalogoMaterialesService.ObtenerMaterialInfo();
            Materiales = new ObservableCollection<MaterialesDto>(datosmateriales);
        }

        private void FiltrarListaCheckboxes(ObservableCollection<OpcionFiltroCheckbox> coleccion, string textoBusqueda)
        {
            ICollectionView vista = CollectionViewSource.GetDefaultView(coleccion);
            if (vista == null) return;

            if (string.IsNullOrWhiteSpace(textoBusqueda))
            {
                vista.Filter = null;
            }
            else
            {
                vista.Filter = item => item is OpcionFiltroCheckbox opcion &&
                                       !string.IsNullOrEmpty(opcion.Nombre) &&
                                       opcion.Nombre.Contains(textoBusqueda, StringComparison.OrdinalIgnoreCase);
            }
        }

        private bool FiltroEjecucion(object item)
        {
            if (item is not MaterialesDto material) return false;

            var descripcionesSeleccionadas = _estadosDescripcion.Where(x => x.Value).Select(x => x.Key).ToList();
            if (descripcionesSeleccionadas.Any() && (string.IsNullOrEmpty(material.DescripcionCorta) || !descripcionesSeleccionadas.Contains(material.DescripcionCorta)))
                return false;

            var marcasSeleccionadas = _estadosMarca.Where(x => x.Value).Select(x => x.Key).ToList();
            if (marcasSeleccionadas.Any() && (string.IsNullOrEmpty(material.Marca) || !marcasSeleccionadas.Contains(material.Marca)))
                return false;

            var noPartesSeleccionadas = _estadosNoParte.Where(x => x.Value).Select(x => x.Key).ToList();
            if (noPartesSeleccionadas.Any() && (string.IsNullOrEmpty(material.NoParte) || !noPartesSeleccionadas.Contains(material.NoParte)))
                return false;

            var clasificacionesSeleccionadas = _estadosClasificacion.Where(x => x.Value).Select(x => x.Key).ToList();
            if (clasificacionesSeleccionadas.Any() && (string.IsNullOrEmpty(material.Clasificacion) || !clasificacionesSeleccionadas.Contains(material.Clasificacion)))
                return false;

            var subclasificacionesSeleccionadas = _estadosSubclasificacion.Where(x => x.Value).Select(x => x.Key).ToList();
            if (subclasificacionesSeleccionadas.Any() && (string.IsNullOrEmpty(material.Subclasificacion) || !subclasificacionesSeleccionadas.Contains(material.Subclasificacion)))
                return false;

            var grupoSeleccionadas = _estadosGrupo.Where(x => x.Value).Select(x => x.Key).ToList();
            if (grupoSeleccionadas.Any() && (string.IsNullOrEmpty(material.Grupo) || !grupoSeleccionadas.Contains(material.Grupo)))
                return false;

            return true;
        }

        private void RecalcularOpcionesFiltros(string? columnaExcluida = null)
        {
            if (_isResetting) return;
            _isResetting = true;

            var itemsVisibles = Materiales.Where(FiltroEjecucion).ToList();

            if (columnaExcluida != "DESCRIPCION")
            {
                var descVisibles = itemsVisibles.Select(e => e.DescripcionCorta).Where(d => !string.IsNullOrEmpty(d)).Distinct().ToList();
                FiltroDescripcionesOpciones.Clear();
                var descAMostrar = _estadosDescripcion.Where(x => x.Value).Select(x => x.Key).Union(descVisibles).Where(d => d != null).OrderBy(x => x);
                foreach (var d in descAMostrar)
                {
                    _estadosDescripcion.TryAdd(d!, false);
                    FiltroDescripcionesOpciones.Add(new OpcionFiltroCheckbox { Nombre = d!, IsChecked = _estadosDescripcion[d!], AlCambiarSeleccion = () => NotificarCheckboxCambiado("DESCRIPCION", d!, null) });
                }
                FiltrarListaCheckboxes(FiltroDescripcionesOpciones, BusquedaDescripcion);
            }

            if (columnaExcluida != "MARCA")
            {
                var marcasVisibles = itemsVisibles.Select(e => e.Marca).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
                FiltroMarcasOpciones.Clear();
                var marcasAMostrar = _estadosMarca.Where(x => x.Value).Select(x => x.Key).Union(marcasVisibles).Distinct();
                foreach (var idM in marcasAMostrar)
                {
                    var nombreM = idM ?? "Sin Marca";
                    _estadosMarca.TryAdd(idM, false);
                    FiltroMarcasOpciones.Add(new OpcionFiltroCheckbox { IdString = idM, Nombre = nombreM, IsChecked = _estadosMarca[idM], AlCambiarSeleccion = () => NotificarCheckboxCambiado("MARCA", null, idM) });
                }
                FiltrarListaCheckboxes(FiltroMarcasOpciones, BusquedaMarca);
            }

            if (columnaExcluida != "NO PARTE")
            {
                var noParteVisibles = itemsVisibles.Select(e => e.NoParte).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
                FiltroNoParteOpciones.Clear();
                var noPartesAMostrar = _estadosNoParte.Where(x => x.Value).Select(x => x.Key).Union(noParteVisibles).Where(s => s != null).OrderBy(x => x);
                foreach (var s in noPartesAMostrar)
                {
                    _estadosNoParte.TryAdd(s!, false);
                    FiltroNoParteOpciones.Add(new OpcionFiltroCheckbox { Nombre = s!, IsChecked = _estadosNoParte[s!], AlCambiarSeleccion = () => NotificarCheckboxCambiado("NO PARTE", s!, null) });
                }
                FiltrarListaCheckboxes(FiltroNoParteOpciones, BusquedaNoParte);
            }

            if (columnaExcluida != "CLASIFICACION")
            {
                var clasificacionesVisibles = itemsVisibles.Select(e => e.Clasificacion).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
                FiltroClasificacionesOpciones.Clear();
                var clasificacionesAMostrar = _estadosClasificacion.Where(x => x.Value).Select(x => x.Key).Union(clasificacionesVisibles).Distinct();
                foreach (var idC in clasificacionesAMostrar)
                {
                    var nombreC = ListaClasificaciones.FirstOrDefault(c => c.Descripcion == idC)?.Descripcion ?? "Sin clasificar";
                    _estadosClasificacion.TryAdd(idC, false);
                    FiltroClasificacionesOpciones.Add(new OpcionFiltroCheckbox { IdString = idC, Nombre = nombreC, IsChecked = _estadosClasificacion[idC], AlCambiarSeleccion = () => NotificarCheckboxCambiado("CLASIFICACION", null, idC) });
                }
                FiltrarListaCheckboxes(FiltroClasificacionesOpciones, BusquedaClasificacion);
            }

            if (columnaExcluida != "SUBCLASIFICACION")
            {
                var subclasificacionesVisibles = itemsVisibles.Select(e => e.Subclasificacion).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
                FiltroSubclasificacionesOpciones.Clear();
                var subclasificacionesAMostrar = _estadosSubclasificacion.Where(x => x.Value).Select(x => x.Key).Union(subclasificacionesVisibles).Distinct();
                foreach (var idS in subclasificacionesAMostrar)
                {
                    var nombreS = ListaSubclasificaciones.FirstOrDefault(s => s.Descripcion == idS)?.Descripcion ?? "Sin subclasificar";
                    _estadosSubclasificacion.TryAdd(idS, false);
                    FiltroSubclasificacionesOpciones.Add(new OpcionFiltroCheckbox { IdString = idS, Nombre = nombreS, IsChecked = _estadosSubclasificacion[idS], AlCambiarSeleccion = () => NotificarCheckboxCambiado("SUBCLASIFICACION", null, idS) });
                }
                FiltrarListaCheckboxes(FiltroSubclasificacionesOpciones, BusquedaSubclasificacion);
            }

            if (columnaExcluida != "GRUPO")
            {
                var gruposVisibles = itemsVisibles.Select(e => e.Grupo).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
                FiltroGrupoOpciones.Clear();
                var gruposAMostrar = _estadosGrupo.Where(x => x.Value).Select(x => x.Key).Union(gruposVisibles).Distinct();
                foreach (var idG in gruposAMostrar)
                {
                    var nombreG = ListaGrupos.FirstOrDefault(g => g.DescripcionGrupo == idG)?.DescripcionGrupo ?? "Sin maquina relacionada";
                    _estadosGrupo.TryAdd(idG, false);
                    FiltroGrupoOpciones.Add(new OpcionFiltroCheckbox { IdString = idG, Nombre = nombreG, IsChecked = _estadosGrupo[idG], AlCambiarSeleccion = () => NotificarCheckboxCambiado("GRUPO", null, idG) });
                }
                FiltrarListaCheckboxes(FiltroGrupoOpciones, BusquedaGrupo);
            }

            _isResetting = false;
        }

        private void NotificarCheckboxCambiado(string columnaOrigen, string? claveTexto, string? claveStringId)
        {
            if (_isResetting) return;

            if (claveTexto != null)
            {
                if (columnaOrigen == "DESCRIPCION" && FiltroDescripcionesOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is { } descOpt)
                    _estadosDescripcion[claveTexto] = descOpt.IsChecked;
                if (columnaOrigen == "NO PARTE" && FiltroNoParteOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is { } serOpt)
                    _estadosNoParte[claveTexto] = serOpt.IsChecked;
            }

            if (claveStringId != null)
            {
                if (columnaOrigen == "MARCA" && FiltroMarcasOpciones.FirstOrDefault(x => x.IdString == claveStringId) is { } marcaOpt)
                    _estadosMarca[claveStringId] = marcaOpt.IsChecked;
                if (columnaOrigen == "CLASIFICACION" && FiltroClasificacionesOpciones.FirstOrDefault(x => x.IdString == claveStringId) is { } ubOpt)
                    _estadosClasificacion[claveStringId] = ubOpt.IsChecked;
                if (columnaOrigen == "SUBCLASIFICACION" && FiltroSubclasificacionesOpciones.FirstOrDefault(x => x.IdString == claveStringId) is { } subOpt)
                    _estadosSubclasificacion[claveStringId] = subOpt.IsChecked;
                if (columnaOrigen == "GRUPO" && FiltroGrupoOpciones.FirstOrDefault(x => x.IdString == claveStringId) is { } grOpt)
                    _estadosGrupo[claveStringId] = grOpt.IsChecked;
            }

            VistaMateriales.Refresh();
            RecalcularOpcionesFiltros(columnaOrigen);
        }

        private void LimpiarFiltros()
        {
            _estadosDescripcion.Clear();
            _estadosMarca.Clear();
            _estadosNoParte.Clear();
            _estadosClasificacion.Clear();
            _estadosSubclasificacion.Clear();
            _estadosGrupo.Clear();

            BusquedaDescripcion = string.Empty;
            BusquedaMarca = string.Empty;
            BusquedaNoParte = string.Empty;
            BusquedaClasificacion = string.Empty;
            BusquedaSubclasificacion = string.Empty;
            BusquedaGrupo = string.Empty;

            RecalcularOpcionesFiltros();
            VistaMateriales.Refresh();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        public RelayCommand(Action execute) => _execute = execute;
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute();
    }
}