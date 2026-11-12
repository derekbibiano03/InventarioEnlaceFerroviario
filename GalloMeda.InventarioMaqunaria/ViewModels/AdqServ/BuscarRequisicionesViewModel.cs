using Inventario.Core;
using Inventario.Core.DTOs.Requisicion;
using Inventario.Core.Services.Adq_Serv.AdquisicionService;
using Inventario.Data.Models;
using Inventario.Desktop.ViewModels.EconomicosViewModel.InfoEconomico;
using Inventario.Desktop.Views.UserControllers.Economicos;
using System.ComponentModel;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Data;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.AdqServ
{
    public class BuscarRequisicionesViewModel : INotifyPropertyChanged
    {
        // Acción que MainWindow escuchará para cambiar a la vista de Orden de Compra
        public Action<string>? GenerarOCAction { get; set; }
        public ICommand GenerarOCCommand { get; }

        private bool _isResetting = false;

        private string _busquedaIdReq = string.Empty;
        private string _busquedaIdEconomico = string.Empty;
        private string _busquedaFecha = string.Empty;
        private string _busquedaUbicacion = string.Empty;
        private string _busquedaEmpresa = string.Empty;
        private string _busquedaEstatus = string.Empty;
        private string _busquedaSolicitante = string.Empty;
        private string _busquedaAtencion = string.Empty;

        private ObservableCollection<RequisicionListDto> _requisiciones = new();
        public ObservableCollection<RequisicionListDto> Requisiciones
        {
            get => _requisiciones;
            set { _requisiciones = value; OnPropertyChanged(); }
        }

        public ICollectionView VistaRequisiciones { get; set; }
        public ICommand LimpiarFiltrosCommand { get; }
        public ICommand VerExcelCommand { get; }
        public ICommand LiquidarCommand { get; }
        public ICommand LiquidarProcesoCommand { get; }
        public ICommand LiqProcesoCommand { get; }
        private readonly Action _onRecargarBusqueda;

        private RequisicionListDto? _requisicionSeleccionada;
        public RequisicionListDto? RequisicionSeleccionada
        {
            get => _requisicionSeleccionada;
            set { _requisicionSeleccionada = value; OnPropertyChanged(); }
        }

        public ICommand ExportarExcelCommand { get; set; }

        public ObservableCollection<Usuario> Usuarios { get; } = new();
        public ObservableCollection<CatalogoUbicacionesProyecto> Ubicaciones { get; } = new();

        private Dictionary<string, bool> _estadosIdReq = new();
        private Dictionary<string, bool> _estadosIdEconomico = new();
        private Dictionary<string, bool> _estadosFecha = new();
        private Dictionary<string, bool> _estadosUbicacion = new();
        private Dictionary<string, bool> _estadosEmpresa = new();
        private Dictionary<string, bool> _estadosEstatus = new();
        private Dictionary<string, bool> _estadosSolicitante = new();
        private Dictionary<string, bool> _estadosAtencion = new();

        public ObservableCollection<OpcionFiltroCheckbox> FiltroIdReqOpciones { get; set; } = new();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroIdEconomicoOpciones { get; set; } = new();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroFechaOpciones { get; set; } = new();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroUbicacionOpciones { get; set; } = new();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroEmpresaOpciones { get; set; } = new();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroEstatusOpciones { get; set; } = new();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroSolicitanteOpciones { get; set; } = new();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroAtencionOpciones { get; set; } = new();

        public string BusquedaIdReq
        {
            get => _busquedaIdReq;
            set { _busquedaIdReq = value; OnPropertyChanged(); FiltrarListaCheckboxes(FiltroIdReqOpciones, _busquedaIdReq); }
        }

        public string BusquedaIdEconomico
        {
            get => _busquedaIdEconomico;
            set { _busquedaIdEconomico = value; OnPropertyChanged(); FiltrarListaCheckboxes(FiltroIdEconomicoOpciones, _busquedaIdEconomico); }
        }

        public string BusquedaFecha
        {
            get => _busquedaFecha;
            set { _busquedaFecha = value; OnPropertyChanged(); FiltrarListaCheckboxes(FiltroFechaOpciones, _busquedaFecha); }
        }

        public string BusquedaUbicacion
        {
            get => _busquedaUbicacion;
            set { _busquedaUbicacion = value; OnPropertyChanged(); FiltrarListaCheckboxes(FiltroUbicacionOpciones, _busquedaUbicacion); }
        }

        public string BusquedaEmpresa
        {
            get => _busquedaEmpresa;
            set { _busquedaEmpresa = value; OnPropertyChanged(); FiltrarListaCheckboxes(FiltroEmpresaOpciones, _busquedaEmpresa); }
        }

        public string BusquedaEstatus
        {
            get => _busquedaEstatus;
            set { _busquedaEstatus = value; OnPropertyChanged(); FiltrarListaCheckboxes(FiltroEstatusOpciones, _busquedaEstatus); }
        }

        public string BusquedaSolicitante
        {
            get => _busquedaSolicitante;
            set { _busquedaSolicitante = value; OnPropertyChanged(); FiltrarListaCheckboxes(FiltroSolicitanteOpciones, _busquedaSolicitante); }
        }

        public string BusquedaAtencion
        {
            get => _busquedaAtencion;
            set { _busquedaAtencion = value; OnPropertyChanged(); FiltrarListaCheckboxes(FiltroAtencionOpciones, _busquedaAtencion); }
        }

        private readonly AdquisicionService _reqService;
        private readonly ExcelExportService _excelService;
        private readonly InventarioContext _context;
        public event EventHandler? RecargaBusqueda;

        public BuscarRequisicionesViewModel(AdquisicionService reqService, InventarioContext context, Action onRecargarBusqueda)
        {
            _reqService = reqService ?? throw new ArgumentNullException(nameof(reqService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _excelService = new ExcelExportService(_context);

            VerExcelCommand = new RelayCommand(_ => AbrirArchivoExcel());
            LimpiarFiltrosCommand = new RelayCommand(_ => LimpiarFiltros());

            VistaRequisiciones = CollectionViewSource.GetDefaultView(Requisiciones);
            VistaRequisiciones.Filter = FiltroEjecucion;

            LiquidarCommand = new RelayCommand(async (obj) => await EjecutarLiquidarAsync());
            LiquidarProcesoCommand = new RelayCommand(async (obj) => await EjecutarLiquidarProcesoAsync());

            ExportarExcelCommand = new RelayCommand(_ => EjecutarExportacion());

            // CORREGIDO: Se inicializa correctamente el comando usando una expresión lambda
            GenerarOCCommand = new RelayCommand(_ => EjecutarGenerarOC());

            _onRecargarBusqueda = onRecargarBusqueda;

            _ = CargarTodasRequisicionesAsync();
        }

        private void EjecutarGenerarOC()
        {
            if (RequisicionSeleccionada == null)
            {
                MessageBox.Show("Por favor seleccione una requisición para generar la orden de compra.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            GenerarOCAction?.Invoke(RequisicionSeleccionada.IdRequisicion);
        }

        private async Task EjecutarLiquidarAsync()
        {
            if (RequisicionSeleccionada == null)
            {
                MessageBox.Show("Por favor seleccione una requisición para liquidar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var resultado = MessageBox.Show($"¿Está seguro de liquidar la requisición {RequisicionSeleccionada.IdRequisicion}?",
                                            "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    MessageBox.Show("La requisición ha sido liquidada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    RecargaBusqueda?.Invoke(this, EventArgs.Empty);
                    _onRecargarBusqueda?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task EjecutarLiquidarProcesoAsync()
        {
            if (RequisicionSeleccionada == null)
            {
                MessageBox.Show("Por favor seleccione una requisición para liquidar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var resultado = MessageBox.Show($"¿Está seguro de liquidar la requisición {RequisicionSeleccionada.IdRequisicion}?",
                                            "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    bool exito = await _reqService.LiquidarProcesoRequisicionAsync(RequisicionSeleccionada.IdRequisicion);

                    if (exito)
                    {
                        MessageBox.Show("La requisición ha sido liquidada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                        RecargaBusqueda?.Invoke(this, EventArgs.Empty);
                        _onRecargarBusqueda?.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void EjecutarExportacion()
        {
            List<RequisicionListDto> itemsVisiblesEnTabla = VistaRequisiciones.Cast<RequisicionListDto>().ToList();
            if (!itemsVisiblesEnTabla.Any())
            {
                System.Windows.MessageBox.Show("No hay registros seleccionados por los filtros actuales para exportar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string[] idsDeEquiposFiltrados = itemsVisiblesEnTabla
                .Select(x => x.IdRequisicion)
                .Where(id => !string.IsNullOrEmpty(id))
                .ToArray()!;

            List<Requisicione> datosCompletos = await _reqService.ObtenerRequisicionesPorIdsAsync(idsDeEquiposFiltrados);

            byte[] archivoExcelBytes = _excelService.GenerarExcelRequisiciones(datosCompletos);

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
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

        public async Task CargarTodasRequisicionesAsync()
        {
            try
            {
                var datosReq = await _reqService.ObtenerTodasRequisicionesAsync();

                Requisiciones.Clear();
                foreach (var dato in datosReq)
                {
                    Requisiciones.Add(dato);
                }

                RecalcularOpcionesFiltros();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"StackTrace: {ex.StackTrace}\n\nMensaje: {ex.Message}", "Depuración de Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool FiltroEjecucion(object item)
        {
            var req = item as RequisicionListDto;
            if (req == null) return false;

            bool resultado = true;

            var idsSeleccionados = _estadosIdReq.Where(x => x.Value).Select(x => x.Key).ToList();
            if (idsSeleccionados.Any())
            {
                resultado = resultado && req.IdRequisicion != null && idsSeleccionados.Contains(req.IdRequisicion);
            }

            var economicosSeleccionados = _estadosIdEconomico.Where(x => x.Value).Select(x => x.Key).ToList();
            if (economicosSeleccionados.Any())
            {
                resultado = resultado && req.IdEconomico != null && economicosSeleccionados.Contains(req.IdEconomico);
            }

            var fechasSeleccionadas = _estadosFecha.Where(x => x.Value).Select(x => x.Key).ToList();
            if (fechasSeleccionadas.Any())
            {
                resultado = resultado && fechasSeleccionadas.Contains(req.FechaTexto);
            }

            var ubicacionesSeleccionadas = _estadosUbicacion.Where(x => x.Value).Select(x => x.Key).ToList();
            if (ubicacionesSeleccionadas.Any())
            {
                resultado = resultado && req.Ubicacion != null && ubicacionesSeleccionadas.Contains(req.Ubicacion);
            }

            var empresasSeleccionadas = _estadosEmpresa.Where(x => x.Value).Select(x => x.Key).ToList();
            if (empresasSeleccionadas.Any())
            {
                resultado = resultado && req.Empresa != null && empresasSeleccionadas.Contains(req.Empresa);
            }

            var estatusSeleccionados = _estadosEstatus.Where(x => x.Value).Select(x => x.Key).ToList();
            if (estatusSeleccionados.Any())
            {
                resultado = resultado && req.Estatus != null && estatusSeleccionados.Contains(req.Estatus);
            }

            var solicitantesSeleccionados = _estadosSolicitante.Where(x => x.Value).Select(x => x.Key).ToList();
            if (solicitantesSeleccionados.Any())
            {
                resultado = resultado && req.Solicitante != null && solicitantesSeleccionados.Contains(req.Solicitante);
            }

            var atencionSeleccionados = _estadosAtencion.Where(x => x.Value).Select(x => x.Key).ToList();
            if (atencionSeleccionados.Any())
            {
                resultado = resultado && req.Atencion != null && atencionSeleccionados.Contains(req.Atencion);
            }

            return resultado;
        }

        private void RecalcularOpcionesFiltros(string? columnaExcluida = null)
        {
            if (_isResetting) return;
            _isResetting = true;

            var itemsVisibles = Requisiciones.Where(FiltroEjecucion).ToList();

            if (columnaExcluida != "ID_REQ")
            {
                var visibles = itemsVisibles.Select(e => e.IdRequisicion).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
                FiltroIdReqOpciones.Clear();
                var aMostrar = _estadosIdReq.Where(x => x.Value).Select(x => x.Key).Union(visibles).Where(id => id != null).OrderBy(x => x);
                foreach (var id in aMostrar)
                {
                    _estadosIdReq.TryAdd(id!, false);
                    FiltroIdReqOpciones.Add(new OpcionFiltroCheckbox { Nombre = id!, IsChecked = _estadosIdReq[id!], AlCambiarSeleccion = () => NotificarCheckboxCambiadoTexto("ID_REQ", id!) });
                }
                FiltrarListaCheckboxes(FiltroIdReqOpciones, BusquedaIdReq);
            }

            if (columnaExcluida != "FECHA")
            {
                var visibles = itemsVisibles.Select(e => e.FechaTexto).Where(f => !string.IsNullOrEmpty(f)).Distinct().ToList();
                FiltroFechaOpciones.Clear();
                var aMostrar = _estadosFecha.Where(x => x.Value).Select(x => x.Key).Union(visibles).Where(f => f != null).OrderBy(x => x);
                foreach (var f in aMostrar)
                {
                    _estadosFecha.TryAdd(f!, false);
                    FiltroFechaOpciones.Add(new OpcionFiltroCheckbox { Nombre = f!, IsChecked = _estadosFecha[f!], AlCambiarSeleccion = () => NotificarCheckboxCambiadoTexto("FECHA", f!) });
                }
                FiltrarListaCheckboxes(FiltroFechaOpciones, BusquedaFecha);
            }

            if (columnaExcluida != "UBICACION")
            {
                var visibles = itemsVisibles.Select(e => e.Ubicacion).Where(u => !string.IsNullOrEmpty(u)).Distinct().ToList();
                FiltroUbicacionOpciones.Clear();
                var aMostrar = _estadosUbicacion.Where(x => x.Value).Select(x => x.Key).Union(visibles).Where(u => u != null).OrderBy(x => x);
                foreach (var ub in aMostrar)
                {
                    _estadosUbicacion.TryAdd(ub!, false);
                    FiltroUbicacionOpciones.Add(new OpcionFiltroCheckbox { Nombre = ub!, IsChecked = _estadosUbicacion[ub!], AlCambiarSeleccion = () => NotificarCheckboxCambiadoTexto("UBICACION", ub!) });
                }
                FiltrarListaCheckboxes(FiltroUbicacionOpciones, BusquedaUbicacion);
            }

            if (columnaExcluida != "EMPRESA")
            {
                var visibles = itemsVisibles.Select(e => e.Empresa).Where(emp => !string.IsNullOrEmpty(emp)).Distinct().ToList();
                FiltroEmpresaOpciones.Clear();
                var aMostrar = _estadosEmpresa.Where(x => x.Value).Select(x => x.Key).Union(visibles).Where(emp => emp != null).OrderBy(x => x);
                foreach (var emp in aMostrar)
                {
                    _estadosEmpresa.TryAdd(emp!, false);
                    FiltroEmpresaOpciones.Add(new OpcionFiltroCheckbox { Nombre = emp!, IsChecked = _estadosEmpresa[emp!], AlCambiarSeleccion = () => NotificarCheckboxCambiadoTexto("EMPRESA", emp!) });
                }
                FiltrarListaCheckboxes(FiltroEmpresaOpciones, BusquedaEmpresa);
            }

            if (columnaExcluida != "ESTATUS")
            {
                var visibles = itemsVisibles.Select(e => e.Estatus).Where(est => !string.IsNullOrEmpty(est)).Distinct().ToList();
                FiltroEstatusOpciones.Clear();
                var aMostrar = _estadosEstatus.Where(x => x.Value).Select(x => x.Key).Union(visibles).Where(est => est != null).OrderBy(x => x);
                foreach (var est in aMostrar)
                {
                    _estadosEstatus.TryAdd(est!, false);
                    FiltroEstatusOpciones.Add(new OpcionFiltroCheckbox { Nombre = est!, IsChecked = _estadosEstatus[est!], AlCambiarSeleccion = () => NotificarCheckboxCambiadoTexto("ESTATUS", est!) });
                }
                FiltrarListaCheckboxes(FiltroEstatusOpciones, BusquedaEstatus);
            }

            if (columnaExcluida != "SOLICITANTE")
            {
                var visibles = itemsVisibles.Select(e => e.Solicitante).Where(sol => !string.IsNullOrEmpty(sol)).Distinct().ToList();
                FiltroSolicitanteOpciones.Clear();
                var aMostrar = _estadosSolicitante.Where(x => x.Value).Select(x => x.Key).Union(visibles).Where(sol => sol != null).OrderBy(x => x);
                foreach (var sol in aMostrar)
                {
                    _estadosSolicitante.TryAdd(sol!, false);
                    FiltroSolicitanteOpciones.Add(new OpcionFiltroCheckbox { Nombre = sol!, IsChecked = _estadosSolicitante[sol!], AlCambiarSeleccion = () => NotificarCheckboxCambiadoTexto("SOLICITANTE", sol!) });
                }
                FiltrarListaCheckboxes(FiltroSolicitanteOpciones, BusquedaSolicitante);
            }

            if (columnaExcluida != "ATENCION")
            {
                var visibles = itemsVisibles.Select(e => e.Atencion).Where(sol => !string.IsNullOrEmpty(sol)).Distinct().ToList();
                FiltroAtencionOpciones.Clear();
                var aMostrar = _estadosAtencion.Where(x => x.Value).Select(x => x.Key).Union(visibles).Where(sol => sol != null).OrderBy(x => x);
                foreach (var sol in aMostrar)
                {
                    _estadosAtencion.TryAdd(sol!, false);
                    FiltroAtencionOpciones.Add(new OpcionFiltroCheckbox { Nombre = sol!, IsChecked = _estadosAtencion[sol!], AlCambiarSeleccion = () => NotificarCheckboxCambiadoTexto("ATENCION", sol!) });
                }
                FiltrarListaCheckboxes(FiltroAtencionOpciones, BusquedaAtencion);
            }

            if (columnaExcluida != "ECONOMICO")
            {
                var visibles = itemsVisibles.Select(e => e.IdEconomico).Where(sol => !string.IsNullOrEmpty(sol)).Distinct().ToList();
                FiltroIdEconomicoOpciones.Clear();

                var aMostrar = _estadosIdEconomico.Where(x => x.Value).Select(x => x.Key).Union(visibles).Where(sol => sol != null).OrderBy(x => x);

                foreach (var sol in aMostrar)
                {
                    _estadosIdEconomico.TryAdd(sol!, false);
                    FiltroIdEconomicoOpciones.Add(new OpcionFiltroCheckbox
                    {
                        Nombre = sol!,
                        IsChecked = _estadosIdEconomico[sol!],
                        AlCambiarSeleccion = () => NotificarCheckboxCambiadoTexto("ECONOMICO", sol!)
                    });
                }

                FiltrarListaCheckboxes(FiltroIdEconomicoOpciones, BusquedaIdEconomico);
            }

            _isResetting = false;
        }

        private void NotificarCheckboxCambiadoTexto(string columnaOrigen, string claveTexto)
        {
            if (_isResetting) return;

            if (columnaOrigen == "ID_REQ" && FiltroIdReqOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is var opt1 && opt1 != null)
                _estadosIdReq[claveTexto] = opt1.IsChecked;

            if (columnaOrigen == "FECHA" && FiltroFechaOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is var opt2 && opt2 != null)
                _estadosFecha[claveTexto] = opt2.IsChecked;

            if (columnaOrigen == "EMPRESA" && FiltroEmpresaOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is var opt3 && opt3 != null)
                _estadosEmpresa[claveTexto] = opt3.IsChecked;

            if (columnaOrigen == "UBICACION" && FiltroUbicacionOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is var optUbi && optUbi != null)
                _estadosUbicacion[claveTexto] = optUbi.IsChecked;

            if (columnaOrigen == "ESTATUS" && FiltroEstatusOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is var opt4 && opt4 != null)
                _estadosEstatus[claveTexto] = opt4.IsChecked;

            if (columnaOrigen == "SOLICITANTE" && FiltroSolicitanteOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is var opt5 && opt5 != null)
                _estadosSolicitante[claveTexto] = opt5.IsChecked;

            if (columnaOrigen == "ATENCION" && FiltroAtencionOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is var opt6 && opt6 != null)
                _estadosAtencion[claveTexto] = opt6.IsChecked;

            if (columnaOrigen == "ECONOMICO" && FiltroIdEconomicoOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is var opt7 && opt7 != null)
                _estadosIdEconomico[claveTexto] = opt7.IsChecked;

            VistaRequisiciones.Refresh();
            RecalcularOpcionesFiltros(columnaOrigen);
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

        private void LimpiarFiltros()
        {
            _isResetting = true;

            _busquedaIdEconomico = string.Empty;
            _busquedaIdReq = string.Empty;
            _busquedaFecha = string.Empty;
            _busquedaUbicacion = string.Empty;
            _busquedaEmpresa = string.Empty;
            _busquedaEstatus = string.Empty;
            _busquedaSolicitante = string.Empty;
            _busquedaAtencion = string.Empty;

            OnPropertyChanged(nameof(BusquedaIdReq));
            OnPropertyChanged(nameof(BusquedaIdEconomico));
            OnPropertyChanged(nameof(BusquedaFecha));
            OnPropertyChanged(nameof(BusquedaUbicacion));
            OnPropertyChanged(nameof(BusquedaEmpresa));
            OnPropertyChanged(nameof(BusquedaEstatus));
            OnPropertyChanged(nameof(BusquedaSolicitante));
            OnPropertyChanged(nameof(BusquedaAtencion));

            _estadosIdReq.Clear();
            _estadosIdEconomico.Clear();
            _estadosFecha.Clear();
            _estadosUbicacion.Clear();
            _estadosEmpresa.Clear();
            _estadosEstatus.Clear();
            _estadosSolicitante.Clear();
            _estadosAtencion.Clear();

            _isResetting = false;

            VistaRequisiciones.Refresh();
            RecalcularOpcionesFiltros();
        }

        private void AbrirArchivoExcel()
        {
            try
            {
                if (RequisicionSeleccionada == null)
                {
                    System.Windows.MessageBox.Show("Por favor, seleccione una requisición de la lista.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(RequisicionSeleccionada.ArchivoReq))
                {
                    System.Windows.MessageBox.Show("La requisición seleccionada no cuenta con un archivo adjunto.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _reqService.DescargarYAbrirArchivoExcel(RequisicionSeleccionada.ArchivoReq);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"DETALLE DEL ERROR:\n\nMensaje: {ex.Message}\n\nStackTrace:\n{ex.StackTrace}",
                                    "Error en AbrirArchivoExcel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}