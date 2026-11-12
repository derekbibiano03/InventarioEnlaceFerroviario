using Inventario.Core;
using Inventario.Core.DTOs;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Inventario.Desktop.Views;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.EconomicosViewModel.InfoEconomico
{
    public class OpcionFiltroSuvCheckbox : INotifyPropertyChanged
    {
        private bool _isChecked;
        public string? Nombre { get; set; }
        public int Id { get; set; }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
                AlCambiarSeleccion?.Invoke();
            }
        }

        public required Action AlCambiarSeleccion { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class EconomicosSuvViewModel : INotifyPropertyChanged
    {
        private bool _isResetting = false;
        private readonly CatalogoEconomicosService _economicosService;
        private readonly InventarioContext _contextoCompartido;
        private readonly ExcelExportService _excelService;

        private string _busquedaId = string.Empty;
        private string _busquedaDescripcion = string.Empty;
        private string _busquedaMarca = string.Empty;
        private string _busquedaSerie = string.Empty;
        private string _busquedaTipoEquipo = string.Empty;
        private string _busquedaUbicacion = string.Empty;
        private string _busquedaTipoSeguro = string.Empty;

        public ICommand ExportarExcelCommand { get; set; }
        public ObservableCollection<EconomicoMinimoDto> Economicos { get; set; }
        public ICollectionView VistaEconomicos { get; set; }
        public ICommand EditarCommand { get; }

        private Dictionary<string, bool> _estadosId = new Dictionary<string, bool>();
        private Dictionary<string, bool> _estadosDescripcion = new Dictionary<string, bool>();
        private Dictionary<int, bool> _estadosMarca = new Dictionary<int, bool>();
        private Dictionary<string, bool> _estadosSerie = new Dictionary<string, bool>();
        private Dictionary<string, bool> _estadosTipoEquipo = new Dictionary<string, bool>();
        private Dictionary<int, bool> _estadosUbicacion = new Dictionary<int, bool>();
        private Dictionary<string, bool> _estadosTipoSeguro = new Dictionary<string, bool>();

        public ObservableCollection<OpcionFiltroCheckbox> FiltroIdOpciones { get; set; } = new ObservableCollection<OpcionFiltroCheckbox>();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroDescripcionesOpciones { get; set; } = new ObservableCollection<OpcionFiltroCheckbox>();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroMarcasOpciones { get; set; } = new ObservableCollection<OpcionFiltroCheckbox>();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroSeriesOpciones { get; set; } = new ObservableCollection<OpcionFiltroCheckbox>();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroTipoEquipoOpciones { get; set; } = new ObservableCollection<OpcionFiltroCheckbox>();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroUbicacionesOpciones { get; set; } = new ObservableCollection<OpcionFiltroCheckbox>();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroTiposSeguroOpciones { get; set; } = new ObservableCollection<OpcionFiltroCheckbox>();

        private ObservableCollection<CatalogoUbicacionesProyecto> _listaUbicaciones = new ObservableCollection<CatalogoUbicacionesProyecto>();

        public ObservableCollection<CatalogoUbicacionesProyecto> ListaUbicaciones
        {
            get => _listaUbicaciones;
            set
            {
                _listaUbicaciones = value;
                OnPropertyChanged();
            }
        }
        public ICommand VerDetalleCommand { get; }
        public ICommand LimpiarFiltrosCommand { get; }

        private string[] _opciones = Array.Empty<string>();

        public string[] Opciones
        {
            get => _opciones;
            set => _opciones = value;
        }

        public string BusquedaId
        {
            get => _busquedaId;
            set
            {
                _busquedaId = value;
                OnPropertyChanged();
                FiltrarListaCheckboxes(FiltroIdOpciones, _busquedaId);
            }
        }

        public string BusquedaDescripcion
        {
            get => _busquedaDescripcion;
            set
            {
                _busquedaDescripcion = value;
                OnPropertyChanged();
                FiltrarListaCheckboxes(FiltroDescripcionesOpciones, _busquedaDescripcion);
            }
        }

        public string BusquedaMarca
        {
            get => _busquedaMarca;
            set
            {
                _busquedaMarca = value;
                OnPropertyChanged();
                FiltrarListaCheckboxes(FiltroMarcasOpciones, _busquedaMarca);
            }
        }

        public string BusquedaSerie
        {
            get => _busquedaSerie;
            set
            {
                _busquedaSerie = value;
                OnPropertyChanged();
                FiltrarListaCheckboxes(FiltroSeriesOpciones, _busquedaSerie);
            }
        }

        public string BusquedaTipoEquipo
        {
            get => _busquedaTipoEquipo;
            set
            {
                _busquedaTipoEquipo = value;
                OnPropertyChanged();
                FiltrarListaCheckboxes(FiltroTipoEquipoOpciones, _busquedaTipoEquipo);
            }
        }

        public string BusquedaUbicacion
        {
            get => _busquedaUbicacion;
            set
            {
                _busquedaUbicacion = value;
                OnPropertyChanged();
                FiltrarListaCheckboxes(FiltroUbicacionesOpciones, _busquedaUbicacion);
            }
        }

        public string BusquedaTipoSeguro
        {
            get => _busquedaTipoSeguro;
            set
            {
                _busquedaTipoSeguro = value;
                OnPropertyChanged();
                FiltrarListaCheckboxes(FiltroTiposSeguroOpciones, _busquedaTipoSeguro);
            }
        }

        public EconomicosSuvViewModel()
        {
            _excelService = new ExcelExportService(_contextoCompartido);
            VerDetalleCommand = new RelayCommand<string>(AbrirVentanaDetalle);
            LimpiarFiltrosCommand = new RelayCommand<object>(x => LimpiarFiltros());
            EditarCommand = new RelayCommand<string>(AbrirVentanaEditar);

            _contextoCompartido = new InventarioContext();
            var logsService = new LogsService(_contextoCompartido);
            _economicosService = new CatalogoEconomicosService(_contextoCompartido, logsService);

            Economicos = new ObservableCollection<EconomicoMinimoDto>();

            VistaEconomicos = CollectionViewSource.GetDefaultView(Economicos);
            VistaEconomicos.Filter = FiltroEjecucion;

            ExportarExcelCommand = new RelayCommand(EjecutarExportacion);

            CargarEconomicos();
        }

        // Método de filtrado real que remueve físicamente de la vista los items no coincidentes
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
                vista.Filter = item =>
                {
                    if (item is OpcionFiltroCheckbox opcion)
                    {
                        return !string.IsNullOrEmpty(opcion.Nombre) &&
                               opcion.Nombre.Contains(textoBusqueda, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                };
            }
        }

        private void AbrirVentanaEditar(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            EditarEconomicoWindow ventanaEditar = new EditarEconomicoWindow(id);
            bool? resultado = ventanaEditar.ShowDialog();
            if (resultado == true)
            {
                CargarEconomicos();
            }
        }

        private void EjecutarExportacion()
        {
            List<EconomicoMinimoDto> equiposVisiblesEnTabla = VistaEconomicos.Cast<EconomicoMinimoDto>().ToList();
            if (!equiposVisiblesEnTabla.Any())
            {
                System.Windows.MessageBox.Show("No hay registros seleccionados por los filtros actuales para exportar.", "Atención", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            string[] idsDeEquiposFiltrados = equiposVisiblesEnTabla.Select(x => x.IdEconomico).Where(id => id != null).Select(id => id!).ToArray();
            List<CatalogoEconomico> datosCompletos = _economicosService.ObtenerEconomicosPorListaDeIds(idsDeEquiposFiltrados);
            byte[] archivoExcelBytes = _excelService.GenerarExcelEconomicos(datosCompletos);

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = $"Reporte_Inventario_Completo_{DateTime.Now:yyyyMMdd}",
                DefaultExt = ".xlsx",
                Filter = "Archivos de Excel (*.xlsx)|*.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, archivoExcelBytes);
            }
        }

        private bool FiltroEjecucion(object item)
        {
            var economico = item as EconomicoMinimoDto;
            if (economico == null) return false;

            bool resultado = true;

            var idsSeleccionados = _estadosId.Where(x => x.Value).Select(x => x.Key).ToList();
            if (idsSeleccionados.Any())
            {
                resultado = resultado && economico.IdEconomico != null && idsSeleccionados.Contains(economico.IdEconomico);
            }

            var descripcionesSeleccionadas = _estadosDescripcion.Where(x => x.Value).Select(x => x.Key).ToList();
            if (descripcionesSeleccionadas.Any())
            {
                resultado = resultado && economico.Descripcion != null && descripcionesSeleccionadas.Contains(economico.Descripcion);
            }

            var marcasSeleccionadas = _estadosMarca.Where(x => x.Value).Select(x => x.Key).ToList();
            if (marcasSeleccionadas.Any())
            {
                resultado = resultado && economico.IdMarca.HasValue && marcasSeleccionadas.Contains(economico.IdMarca.Value);
            }

            var seriesSeleccionadas = _estadosSerie.Where(x => x.Value).Select(x => x.Key).ToList();
            if (seriesSeleccionadas.Any())
            {
                resultado = resultado && economico.Serie != null && seriesSeleccionadas.Contains(economico.Serie);
            }

            var tipoequiposSeleccionadas = _estadosTipoEquipo.Where(x => x.Value).Select(x => x.Key).ToList();
            if (tipoequiposSeleccionadas.Any())
            {
                resultado = resultado && economico.IdTipoEquipo != null && tipoequiposSeleccionadas.Contains(economico.IdTipoEquipo);
            }

            var ubicacionesSeleccionadas = _estadosUbicacion.Where(x => x.Value).Select(x => x.Key).ToList();
            if (ubicacionesSeleccionadas.Any())
            {
                resultado = resultado && economico.IdUbicacion.HasValue && ubicacionesSeleccionadas.Contains(economico.IdUbicacion.Value);
            }

            var tipoSeguroSeleccionadas = _estadosTipoSeguro.Where(x => x.Value).Select(x => x.Key).ToList();
            if (tipoSeguroSeleccionadas.Any())
            {
                resultado = resultado && economico.TipoSeguro != null && tipoSeguroSeleccionadas.Contains(economico.TipoSeguro);
            }

            return resultado;
        }

        private void RecalcularOpcionesFiltros(string? columnaExcluida = null)
        {
            if (_isResetting) return;
            _isResetting = true;
            var itemsVisibles = Economicos.Where(FiltroEjecucion).ToList();

            if (columnaExcluida != "ID")
            {
                var idsVisibles = itemsVisibles.Select(e => e.IdEconomico).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
                FiltroIdOpciones.Clear();
                var idsAMostrar = _estadosId.Where(x => x.Value).Select(x => x.Key).Union(idsVisibles).Where(id => id != null).OrderBy(x => x);
                foreach (var id in idsAMostrar)
                {
                    _estadosId.TryAdd(id!, false);
                    FiltroIdOpciones.Add(new OpcionFiltroCheckbox { Nombre = id!, IsChecked = _estadosId[id!], AlCambiarSeleccion = () => NotificarCheckboxCambiado("ID", id!, null) });
                }
                FiltrarListaCheckboxes(FiltroIdOpciones, BusquedaId);
            }

            if (columnaExcluida != "DESCRIPCION")
            {
                var descVisibles = itemsVisibles.Select(e => e.Descripcion).Where(d => !string.IsNullOrEmpty(d)).Distinct().ToList();
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
                var marcasVisibles = itemsVisibles.Where(e => e.IdMarcaNavigation != null && !string.IsNullOrEmpty(e.IdMarcaNavigation.NombreMarca))
                    .Select(e => new { Id = e.IdMarca!.Value, Nombre = e.IdMarcaNavigation!.NombreMarca }).Distinct().ToList();
                FiltroMarcasOpciones.Clear();
                var marcasAMostrarIds = _estadosMarca.Where(x => x.Value).Select(x => x.Key).Union(marcasVisibles.Select(v => v.Id)).Distinct();
                foreach (var idM in marcasAMostrarIds)
                {
                    var elementoEncontrado = Economicos.FirstOrDefault(e => e.IdMarca == idM);
                    var nombreM = elementoEncontrado?.IdMarcaNavigation?.NombreMarca ?? "Desconocido";

                    _estadosMarca.TryAdd(idM, false);
                    FiltroMarcasOpciones.Add(new OpcionFiltroCheckbox { Id = idM, Nombre = nombreM, IsChecked = _estadosMarca[idM], AlCambiarSeleccion = () => NotificarCheckboxCambiado("MARCA", null, idM) });
                }
                FiltrarListaCheckboxes(FiltroMarcasOpciones, BusquedaMarca);
            }

            if (columnaExcluida != "SERIE")
            {
                var seriesVisibles = itemsVisibles.Select(e => e.Serie).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
                FiltroSeriesOpciones.Clear();
                var seriesAMostrar = _estadosSerie.Where(x => x.Value).Select(x => x.Key).Union(seriesVisibles).Where(s => s != null).OrderBy(x => x);
                foreach (var s in seriesAMostrar)
                {
                    _estadosSerie.TryAdd(s!, false);
                    FiltroSeriesOpciones.Add(new OpcionFiltroCheckbox { Nombre = s!, IsChecked = _estadosSerie[s!], AlCambiarSeleccion = () => NotificarCheckboxCambiado("SERIE", s!, null) });
                }
                FiltrarListaCheckboxes(FiltroSeriesOpciones, BusquedaSerie);
            }

            if (columnaExcluida != "TIPO_EQUIPO")
            {
                var tiposVisibles = itemsVisibles.Select(e => e.IdTipoEquipo).Where(t => !string.IsNullOrEmpty(t)).Distinct().ToList();
                FiltroTipoEquipoOpciones.Clear();
                var tiposAMostrar = _estadosTipoEquipo.Where(x => x.Value).Select(x => x.Key).Union(tiposVisibles).Where(t => t != null).OrderBy(x => x);
                foreach (var t in tiposAMostrar)
                {
                    _estadosTipoEquipo.TryAdd(t!, false);
                    FiltroTipoEquipoOpciones.Add(new OpcionFiltroCheckbox { Nombre = t!, IsChecked = _estadosTipoEquipo[t!], AlCambiarSeleccion = () => NotificarCheckboxCambiado("TIPO_EQUIPO", t!, null) });
                }
                FiltrarListaCheckboxes(FiltroTipoEquipoOpciones, BusquedaTipoEquipo);
            }

            if (columnaExcluida != "UBICACION")
            {
                var ubicacionesVisibles = itemsVisibles.Where(e => e.IdUbicacionNavigation != null && !string.IsNullOrEmpty(e.IdUbicacionNavigation.NombreProyecto))
                    .Select(e => new { Id = e.IdUbicacion!.Value, Nombre = e.IdUbicacionNavigation!.NombreProyecto }).Distinct().ToList();
                FiltroUbicacionesOpciones.Clear();
                var ubicacionesAMostrarIds = _estadosUbicacion.Where(x => x.Value).Select(x => x.Key).Union(ubicacionesVisibles.Select(v => v.Id)).Distinct();
                foreach (var idM in ubicacionesAMostrarIds)
                {
                    var elementoEncontrado = Economicos.FirstOrDefault(e => e.IdUbicacion == idM);
                    var nombreM = elementoEncontrado?.IdUbicacionNavigation?.NombreProyecto ?? "Desconocido";

                    _estadosUbicacion.TryAdd(idM, false);
                    FiltroUbicacionesOpciones.Add(new OpcionFiltroCheckbox { Id = idM, Nombre = nombreM, IsChecked = _estadosUbicacion[idM], AlCambiarSeleccion = () => NotificarCheckboxCambiado("UBICACION", null, idM) });
                }
                FiltrarListaCheckboxes(FiltroUbicacionesOpciones, BusquedaUbicacion);
            }

            if (columnaExcluida != "TIPO_SEGURO")
            {
                var tipoSeguroVisibles = itemsVisibles.Select(e => e.TipoSeguro).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
                FiltroTiposSeguroOpciones.Clear();
                var tiposSegurosAMostrar = _estadosTipoSeguro.Where(x => x.Value).Select(x => x.Key).Union(tipoSeguroVisibles).Where(s => s != null).OrderBy(x => x);
                foreach (var s in tiposSegurosAMostrar)
                {
                    _estadosTipoSeguro.TryAdd(s!, false);
                    FiltroTiposSeguroOpciones.Add(new OpcionFiltroCheckbox { Nombre = s!, IsChecked = _estadosTipoSeguro[s!], AlCambiarSeleccion = () => NotificarCheckboxCambiado("TIPO_SEGURO", s!, null) });
                }
                FiltrarListaCheckboxes(FiltroTiposSeguroOpciones, BusquedaTipoSeguro);
            }

            _isResetting = false;
        }

        private void NotificarCheckboxCambiado(string columnaOrigen, string claveTexto, int? claveId)
        {
            if (_isResetting) return;

            if (claveTexto != null)
            {
                if (columnaOrigen == "ID" && FiltroIdOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is var idOpt && idOpt != null) _estadosId[claveTexto] = idOpt.IsChecked;
                if (columnaOrigen == "DESCRIPCION" && FiltroDescripcionesOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is var descOpt && descOpt != null) _estadosDescripcion[claveTexto] = descOpt.IsChecked;
                if (columnaOrigen == "SERIE" && FiltroSeriesOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is var serOpt && serOpt != null) _estadosSerie[claveTexto] = serOpt.IsChecked;
                if (columnaOrigen == "TIPO_EQUIPO" && FiltroTipoEquipoOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is var tipoOpt && tipoOpt != null) _estadosTipoEquipo[claveTexto] = tipoOpt.IsChecked;
                if (columnaOrigen == "TIPO_SEGURO" && FiltroTiposSeguroOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is var tipsegOpt && tipsegOpt != null) _estadosTipoSeguro[claveTexto] = tipsegOpt.IsChecked;
            }

            if (claveId.HasValue)
            {
                if (columnaOrigen == "MARCA" && FiltroMarcasOpciones.FirstOrDefault(x => x.Id == claveId.Value) is var marcaOpt && marcaOpt != null) _estadosMarca[claveId.Value] = marcaOpt.IsChecked;
                if (columnaOrigen == "UBICACION" && FiltroUbicacionesOpciones.FirstOrDefault(x => x.Id == claveId.Value) is var ubOpt && ubOpt != null) _estadosUbicacion[claveId.Value] = ubOpt.IsChecked;
            }

            VistaEconomicos.Refresh();
            RecalcularOpcionesFiltros(columnaOrigen);
        }

        private void LimpiarFiltros()
        {
            _isResetting = true;

            _busquedaId = string.Empty;
            _busquedaDescripcion = string.Empty;
            _busquedaMarca = string.Empty;
            _busquedaSerie = string.Empty;
            _busquedaTipoEquipo = string.Empty;
            _busquedaUbicacion = string.Empty;
            _busquedaTipoSeguro = string.Empty;

            OnPropertyChanged(nameof(BusquedaId));
            OnPropertyChanged(nameof(BusquedaDescripcion));
            OnPropertyChanged(nameof(BusquedaMarca));
            OnPropertyChanged(nameof(BusquedaSerie));
            OnPropertyChanged(nameof(BusquedaTipoEquipo));
            OnPropertyChanged(nameof(BusquedaUbicacion));
            OnPropertyChanged(nameof(BusquedaTipoSeguro));

            _estadosId.Clear();
            _estadosDescripcion.Clear();
            _estadosMarca.Clear();
            _estadosSerie.Clear();
            _estadosTipoEquipo.Clear();
            _estadosUbicacion.Clear();
            _estadosTipoSeguro.Clear();

            _isResetting = false;

            VistaEconomicos.Refresh();
            RecalcularOpcionesFiltros();
        }

        private void AbrirVentanaDetalle(string id)
        {
            DetallesWindow ventanaDetalle = new DetallesWindow(id);
            ventanaDetalle.ShowDialog();
        }

        public void CargarEconomicos()
        {
            var ubicacionesBBDD = _contextoCompartido.CatalogoUbicacionesProyectos.ToList();
            ListaUbicaciones = new ObservableCollection<CatalogoUbicacionesProyecto>(ubicacionesBBDD);

            var datosBBDD = _economicosService.ObtenerSUVCortos();

            Economicos.Clear();
            foreach (var item in datosBBDD)
            {
                Economicos.Add(item);
            }

            RecalcularOpcionesFiltros();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
