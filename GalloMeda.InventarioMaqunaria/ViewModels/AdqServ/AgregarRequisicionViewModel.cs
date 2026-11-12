using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using GalloMeda.InventarioMaqunaria;
using Inventario.Core.Services.Adq_Serv.AdquisicionService;
using Inventario.Core.Services.Auth;
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

namespace Inventario.Desktop.ViewModels.AdqServ
{
    public class AgregarRequisicionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<DetalleRequisicion> Detalles { get; } = new();
        public ObservableCollection<CatalogoUbicacionesProyecto> Ubicaciones { get; } = new();
        public ObservableCollection<CatalogoEconomico> Economicos { get; } = new();
        public ObservableCollection<Usuario> UsuariosCompras { get; } = new();
        public ObservableCollection<Usuario> UsuariosAutorizantes { get; } = new();

        private readonly InventarioContext _contexto;
        private readonly UsuariosService _usuariosService;
        private readonly Action _onRegistroRequisicionCompletado;

        public ICommand AñadirConceptoCommand { get; }
        public ICommand EliminarConceptoCommand { get; }
        public ICommand GenerarExcelCommand { get; }

        private Usuario? _usuarioAtencionSeleccionado;
        public Usuario? UsuarioAtencionSeleccionado
        {
            get => _usuarioAtencionSeleccionado;
            set
            {
                if (_usuarioAtencionSeleccionado != value)
                {
                    _usuarioAtencionSeleccionado = value;
                    OnPropertyChanged();
                    AtencionNombre = value?.NombreCompleto;
                    AtencionDepto = value?.Area;
                }
            }
        }

        private Usuario? _usuarioAutorizanteSeleccionado;
        public Usuario? UsuarioAutorizanteSeleccionado
        {
            get => _usuarioAutorizanteSeleccionado;
            set
            {
                if (_usuarioAutorizanteSeleccionado != value)
                {
                    _usuarioAutorizanteSeleccionado = value;
                    OnPropertyChanged();
                    FirmaAutorizante = value?.NombreCompleto;
                    CorreoE = value?.Correoe; // <-- CORREGIDO AQUÍ (Correoe en minúscula)
                }
            }
        }

        private string? _ubicacionSeleccionada;
        public string? UbicacionSeleccionada
        {
            get => _ubicacionSeleccionada;
            set
            {
                if (_ubicacionSeleccionada != value)
                {
                    _ubicacionSeleccionada = value;
                    OnPropertyChanged();
                    ActualizarConsecutivo();
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
                    ActualizarConsecutivo();
                }
            }
        }

        private string? _gradoPrioridadSeleccionado;
        public string? GradoPrioridadSeleccionado
        {
            get => _gradoPrioridadSeleccionado;
            set
            {
                if (_gradoPrioridadSeleccionado != value)
                {
                    _gradoPrioridadSeleccionado = value;
                    OnPropertyChanged();
                    ActualizarConsecutivo();
                }
            }
        }

        private string _tipoRequiSeleccionada;
        public string TipoRequiSeleccionada
        {
            get => _tipoRequiSeleccionada;
            set
            {
                _tipoRequiSeleccionada = value;
                OnPropertyChanged(nameof(TipoRequiSeleccionada));

                // Si la requisición no es para económico, vaciamos todos los datos de la unidad
                if (_tipoRequiSeleccionada != "REQUISICION PARA ECONOMICO")
                {
                    EconomicoSeleccionado = null;
                    DescripcionUnidad = null;
                    ModeloUnidad = null;
                    MarcaUnidad = null;
                    SerieUnidad = null;
                    MotorUnidad = null;
                    MotorModelo = null;
                    MotorMarca = null;
                    MotorSerie = null;
                }
            }
        }

        private DateTime _fechaActual = DateTime.Now;
        public DateTime FechaActual
        {
            get => _fechaActual;
            set
            {
                if (_fechaActual != value)
                {
                    _fechaActual = value;
                    OnPropertyChanged();
                    ActualizarConsecutivo();
                }
            }
        }

        private string? _atencionNombre;
        public string? AtencionNombre
        {
            get => _atencionNombre;
            set { _atencionNombre = value; OnPropertyChanged(); }
        }

        private string? _atencionDepto;
        public string? AtencionDepto
        {
            get => _atencionDepto;
            set { _atencionDepto = value; OnPropertyChanged(); }
        }

        private string? _consecutivoGenerado;
        public string? ConsecutivoGenerado
        {
            get => _consecutivoGenerado;
            set { _consecutivoGenerado = value; OnPropertyChanged(); }
        }


        private string? _observaciones;
        public string? Observaciones
        {
            get => _observaciones;
            set { _observaciones = value; OnPropertyChanged(); }
        }

        private string? _firmaAutorizante;
        public string? FirmaAutorizante
        {
            get => _firmaAutorizante;
            set { _firmaAutorizante = value; OnPropertyChanged(); }
        }

        private string? _correoE;
        public string? CorreoE
        {
            get => _correoE;
            set { _correoE = value; OnPropertyChanged(); }
        }

        private string? _descripcionUnidad;
        public string? DescripcionUnidad
        {
            get => _descripcionUnidad;
            set { _descripcionUnidad = value; OnPropertyChanged(); }
        }

        private string? _modeloUnidad;
        public string? ModeloUnidad
        {
            get => _modeloUnidad;
            set { _modeloUnidad = value; OnPropertyChanged(); }
        }

        private string? _motorUnidad;
        public string? MotorUnidad
        {
            get => _motorUnidad;
            set { _motorUnidad = value; OnPropertyChanged(); }
        }

        private string? _motorModelo;
        public string? MotorModelo
        {
            get => _motorModelo;
            set { _motorModelo = value; OnPropertyChanged(); }
        }

        private string? _motorMarca;
        public string? MotorMarca
        {
            get => _motorMarca;
            set { _motorMarca = value; OnPropertyChanged(); }
        }

        private string? _motorSerie;
        public string? MotorSerie
        {
            get => _motorSerie;
            set { _motorSerie = value; OnPropertyChanged(); }
        }

        private string? _marcaUnidad;
        public string? MarcaUnidad
        {
            get => _marcaUnidad;
            set { _marcaUnidad = value; OnPropertyChanged(); }
        }

        private string? _serieUnidad;
        public string? SerieUnidad
        {
            get => _serieUnidad;
            set { _serieUnidad = value; OnPropertyChanged(); }
        }




        public ObservableCollection<CatalogoMarca> ListaMarcas { get; set; } = new();
        public ObservableCollection<CatalogoTiposCombustible> ListaMotores { get; set; } = new();
        public ICollectionView VistaRequisicion { get; set; }

        private CatalogoEconomico? _economicoSeleccionado;
        public CatalogoEconomico? EconomicoSeleccionado
        {
            get => _economicoSeleccionado;
            set
            {
                if (_economicoSeleccionado != value)
                {
                    _economicoSeleccionado = value;
                    OnPropertyChanged();
                    ActualizarDatosEconomico();
                }
            }
        }

        private void ActualizarConsecutivo()
        {
            ConsecutivoGenerado = $"{EmpresaSeleccionada} - RC - 000 - {UbicacionSeleccionada} - {FechaActual:yyyy}";
        }

        private void ActualizarDatosEconomico()
        {
            if (_economicoSeleccionado != null)
            {
                var marcaEncontrada = ListaMarcas?.FirstOrDefault(m => m.IdMarca == _economicoSeleccionado.IdMarca);
                var motorMarcaEncontrado = ListaMarcas?.FirstOrDefault(m => m.IdMarca == _economicoSeleccionado.MarcaMotor);
                var tipoMotor = ListaMotores?.FirstOrDefault(m => m.IdCombustible == _economicoSeleccionado.IdCombustible);

                DescripcionUnidad = _economicoSeleccionado.Descripcion ?? string.Empty;
                ModeloUnidad = _economicoSeleccionado.Modelo ?? string.Empty;
                MotorUnidad = tipoMotor?.DescripcionCombustible ?? string.Empty;
                MotorMarca = motorMarcaEncontrado?.NombreMarca ?? string.Empty;
                MarcaUnidad = marcaEncontrada?.NombreMarca ?? string.Empty;
                SerieUnidad = _economicoSeleccionado.Serie ?? string.Empty;
                MotorModelo = _economicoSeleccionado.ModeloMotor ?? string.Empty;
                MotorSerie = _economicoSeleccionado.SerieMotor ?? string.Empty;
            }
            else
            {
                DescripcionUnidad = string.Empty;
                ModeloUnidad = string.Empty;
                MotorUnidad = string.Empty;
                MarcaUnidad = string.Empty;
                SerieUnidad = string.Empty;
                MotorMarca = string.Empty;
                MotorModelo = string.Empty;
                MotorSerie = string.Empty;
            }
        }

        private readonly AdquisicionService _adquisicionService;
        public event EventHandler? RequisicionRegistrada;
        public AgregarRequisicionViewModel(
            InventarioContext contexto, 
            UsuariosService usuariosService, 
            AdquisicionService adquisicionService, 
            Action onRegistroRequisicionCompletado)
        {
            _usuariosService = usuariosService ?? throw new ArgumentNullException(nameof(usuariosService));
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
            _adquisicionService = adquisicionService ?? throw new ArgumentNullException(nameof(adquisicionService));

            AñadirConceptoCommand = new RelayCommand(EjecutarAñadirConcepto);
            EliminarConceptoCommand = new RelayCommand(EjecutarEliminarConcepto);
            GenerarExcelCommand = new RelayCommand(EjecutarGenerarExcel);
            _onRegistroRequisicionCompletado = onRegistroRequisicionCompletado;

            CargarUsuarios();
            CargarCatalogos();
        }

        public void CargarUsuarios()
        {
            UsuariosCompras.Clear();
            var datosusuarioscompras = _usuariosService.ObtenerUsuariosCompras();
            if (datosusuarioscompras != null)
            {
                foreach (var usuario in datosusuarioscompras)
                {
                    UsuariosCompras.Add(usuario);
                }
            }

            UsuariosAutorizantes.Clear();
            var datosusuariosautorizantes = _usuariosService.ObtenerUsuariosAutorizantes();
            if (datosusuarioscompras != null)
            {
                foreach (var usuario in datosusuariosautorizantes)
                {
                    UsuariosAutorizantes.Add(usuario);
                }
            }
        }

        private void EjecutarEliminarConcepto(object? parametro)
        {
            if (parametro is DetalleRequisicion detalle)
            {
                Detalles.Remove(detalle);
                ReordenarPartidas();
            }
        }

        private void ReordenarPartidas()
        {
            int index = 1;
            foreach (var item in Detalles)
            {
                item.Partida = index++;
            }
        }

        private void EjecutarAñadirConcepto(object? obj)
        {
            int siguientePartida = Detalles.Count + 1;
            Detalles.Add(new DetalleRequisicion { Partida = siguientePartida, Cantidad = 1 });
        }

        private void EjecutarGenerarExcel(object? obj)
        {
            try
            {
                if (EmpresaSeleccionada == null || (EmpresaSeleccionada == "")) // Ajusta según el tipo de dato de tu IdEconomico
                {
                    MessageBox.Show("Seleccione una empresa para la requisicion.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (GradoPrioridadSeleccionado == null || (GradoPrioridadSeleccionado == "")) // Ajusta según el tipo de dato de tu IdEconomico
                {
                    MessageBox.Show("Seleccione un grado de prioridad para generar la requisicion.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (UbicacionSeleccionada == null || (UbicacionSeleccionada == "")) // Ajusta según el tipo de dato de tu IdEconomico
                {
                    MessageBox.Show("Seleccione una ubicacion para la requisicion.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (TipoRequiSeleccionada == null || (TipoRequiSeleccionada == "")) // Ajusta según el tipo de dato de tu IdEconomico
                {
                    MessageBox.Show("Seleccione un tipo de requisicion.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (UsuarioAtencionSeleccionado == null)
                {
                    MessageBox.Show("Seleccione a una persona para que atienda la requisicion.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (Detalles == null || Detalles.Count == 0)
                {
                    MessageBox.Show("Debe agregar al menos un concepto en la orden de compra.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                if (UsuarioAutorizanteSeleccionado == null)
                {
                    MessageBox.Show("Seleccione a una persona para que autorize la requisicion.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                int idUbicacionReal = Ubicaciones.FirstOrDefault(u => u.Siglas == UbicacionSeleccionada)?.IdUbicacion ?? 0;

                // 1. Calculamos el consecutivo primero para que exista en el contexto actual
                int ultimoConsecutivoTemp = _contexto.Requisiciones
                    .Where(r => r.Empresa == EmpresaSeleccionada && r.IdUbicacion == idUbicacionReal)
                    .Select(r => (int?)r.Consecutivo)
                    .Max() ?? 0;

                string nombreArchivoRemoto = $"{EmpresaSeleccionada} - RC - {(ultimoConsecutivoTemp + 1):D3} - {UbicacionSeleccionada} - {FechaActual:yyyy}.xlsx";
                string rutaTemporal = Path.Combine(Path.GetTempPath(), nombreArchivoRemoto);

                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("Requisición");
                    ws.ShowGridLines = true;

                    // Desproteger la hoja y el libro para garantizar edición abierta
                    ws.Unprotect();
                    workbook.Unprotect();

                    ws.Column(1).Width = 8;
                    ws.Column(2).Width = 8.78;
                    ws.Column(3).Width = 8.67;
                    ws.Column(4).Width = 46.44;
                    ws.Column(5).Width = 17.56;
                    ws.Column(6).Width = 14;
                    ws.Column(7).Width = 14;
                    ws.Column(8).Width = 13;

                    var rangoIcono = ws.Range("A1:C1");
                    rangoIcono.Merge();

                    string imagePath = string.Empty;
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;

                    if (EmpresaSeleccionada == "CGM")
                    {
                        imagePath = Path.Combine(basePath, "Resources", "gallo_meda_icon.png");
                    }
                    else if (EmpresaSeleccionada == "OX")
                    {
                        imagePath = Path.Combine(basePath, "Resources", "grupo_ox.jpeg");
                    }

                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        ws.AddPicture(imagePath).MoveTo(ws.Cell("A1")).WithSize(150, 50);
                    }
                    else
                    {
                        MessageBox.Show($"No se encontró la imagen en la ruta: {imagePath}", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                    string folioDinamico = $"{EmpresaSeleccionada} - RC - {(ultimoConsecutivoTemp + 1):D3} - {UbicacionSeleccionada} - {FechaActual:yyyy}";

                    var rangoTitulo = ws.Range("E1:G1");
                    rangoTitulo.Merge();
                    rangoTitulo.FirstCell().Value = "REQUISICIÓN DE COMPRA";
                    rangoTitulo.FirstCell().Style.Font.Bold = true;
                    rangoTitulo.FirstCell().Style.Font.FontSize = 16;
                    rangoTitulo.FirstCell().Style.Font.FontColor = XLColor.FromHtml("#1F4E78");

                    var rangoSub = ws.Range("E2:H2");
                    rangoSub.Merge();
                    rangoSub.FirstCell().Value = folioDinamico;
                    rangoSub.FirstCell().Style.Font.Italic = true;

                    var rangoUbi = ws.Range("E3:H3");
                    rangoUbi.Merge();
                    rangoUbi.FirstCell().Value = FechaActual;
                    rangoUbi.FirstCell().Style.DateFormat.Format = "dd/MM/yyyy";
                    rangoUbi.FirstCell().Style.Font.Italic = true;

                    var rangoInfo = ws.Range("A4:C4");
                    rangoInfo.Merge();
                    rangoInfo.FirstCell().Value = "INFORMACIÓN GENERAL";
                    rangoInfo.FirstCell().Style.Font.Bold = true;
                    rangoInfo.FirstCell().Style.Font.FontColor = XLColor.FromHtml("#1F4E78");

                    var r5_ab = ws.Range("A5:B5"); r5_ab.Merge(); r5_ab.FirstCell().Value = "Atención A:";
                    var r5_cd = ws.Range("C5:D5"); r5_cd.Merge(); r5_cd.FirstCell().Value = AtencionNombre ?? string.Empty;

                    ws.Cell("E5").Value = "Depto. Atención:";
                    var r5_fgh = ws.Range("F5:H5"); r5_fgh.Merge(); r5_fgh.FirstCell().Value = AtencionDepto ?? string.Empty;

                    var r6_ab = ws.Range("A6:B6"); r6_ab.Merge(); r6_ab.FirstCell().Value = "Solicitante:";
                    var r6_cd = ws.Range("C6:D6"); r6_cd.Merge(); r6_cd.FirstCell().Value = App.Session?.NombreCompleto ?? string.Empty;

                    ws.Cell("E6").Value = "Depto. Solicitante:";
                    var r6_fgh = ws.Range("F6:H6"); r6_fgh.Merge(); r6_fgh.FirstCell().Value = App.Session?.Area ?? string.Empty;

                    var rangoEquipo = ws.Range("A9:C9");
                    rangoEquipo.Merge();
                    rangoEquipo.FirstCell().Value = "DATOS DEL EQUIPO O UNIDAD";
                    rangoEquipo.FirstCell().Style.Font.Bold = true;
                    rangoEquipo.FirstCell().Style.Font.FontColor = XLColor.FromHtml("#1F4E78");

                    var r10_ab = ws.Range("A10:B10"); r10_ab.Merge(); r10_ab.FirstCell().Value = "No. Económico:";
                    var r10_cd = ws.Range("C10:D10");
                    r10_cd.Merge();
                    r10_cd.FirstCell().Value = (TipoRequiSeleccionada != "REQUISICION PARA ECONOMICO")
                        ? string.Empty
                        : (EconomicoSeleccionado?.IdEconomico ?? string.Empty);

                    var r11_ab = ws.Range("A11:B11"); r11_ab.Merge(); r11_ab.FirstCell().Value = "Descripcion:";
                    var r11_cd = ws.Range("C11:D11");
                    r11_cd.Merge();
                    r11_cd.FirstCell().Value = (TipoRequiSeleccionada != "REQUISICION PARA ECONOMICO")
                        ? string.Empty
                        : DescripcionUnidad;

                    var r12_ab = ws.Range("A12:B12"); r12_ab.Merge(); r12_ab.FirstCell().Value = "Marca:";
                    var r12_cd = ws.Range("C12:D12");
                    r12_cd.Merge();
                    r12_cd.FirstCell().Value = (TipoRequiSeleccionada != "REQUISICION PARA ECONOMICO")
                        ? string.Empty
                        : MarcaUnidad;

                    var r13_ab = ws.Range("A13:B13"); r13_ab.Merge(); r13_ab.FirstCell().Value = "Modelo:";
                    var r13_cd = ws.Range("C13:D13");
                    r13_cd.Merge();
                    r13_cd.FirstCell().Value = (TipoRequiSeleccionada != "REQUISICION PARA ECONOMICO")
                        ? string.Empty
                        : ModeloUnidad;

                    var r14_ab = ws.Range("A14:B14"); r14_ab.Merge(); r14_ab.FirstCell().Value = "No. Serie:";
                    var r14_cd = ws.Range("C14:D14");
                    r14_cd.Merge();
                    r14_cd.FirstCell().Value = (TipoRequiSeleccionada != "REQUISICION PARA ECONOMICO")
                        ? string.Empty
                        : SerieUnidad;

                    ws.Cell("E10").Value = "Tipo de Motor:";
                    var r10_fgh = ws.Range("F10:H10");
                    r10_fgh.Merge();
                    r10_fgh.FirstCell().Value = (TipoRequiSeleccionada != "REQUISICION PARA ECONOMICO")
                        ? string.Empty
                        : MotorUnidad;

                    ws.Cell("E11").Value = "Marca de Motor:";
                    var r11_fgh = ws.Range("F11:H11");
                    r11_fgh.Merge();
                    r11_fgh.FirstCell().Value = (TipoRequiSeleccionada != "REQUISICION PARA ECONOMICO")
                        ? string.Empty
                        : MotorMarca;

                    ws.Cell("E12").Value = "Modelo de Motor:";
                    var r12_fgh = ws.Range("F12:H12");
                    r12_fgh.Merge();
                    r12_fgh.FirstCell().Value = (TipoRequiSeleccionada != "REQUISICION PARA ECONOMICO")
                        ? string.Empty
                        : MotorModelo;

                    ws.Cell("E13").Value = "Serie de Motor:";
                    var r13_fgh = ws.Range("F13:H13");
                    r13_fgh.Merge();
                    r13_fgh.FirstCell().Value = (TipoRequiSeleccionada != "REQUISICION PARA ECONOMICO")
                        ? string.Empty
                        : MotorSerie;

                    var rangoConceptos = ws.Range("A16:C16");
                    rangoConceptos.Merge();
                    rangoConceptos.FirstCell().Value = "CONCEPTOS / PARTIDAS";
                    rangoConceptos.FirstCell().Style.Font.Bold = true;
                    rangoConceptos.FirstCell().Style.Font.FontColor = XLColor.FromHtml("#1F4E78");

                    string[] headers = { "Partida", "Cantidad", "Unidad", "Descripción", "No. Parte", "No. Equivalente", "Conj", "SubConj" };
                    int headerRow = 17;

                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = ws.Cell(headerRow, i + 1);
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.FontColor = XLColor.White;
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    int rowIdx = 18;
                    foreach (var det in Detalles)
                    {
                        ws.Cell(rowIdx, 1).Value = det.Partida;
                        ws.Cell(rowIdx, 2).Value = det.Cantidad;
                        ws.Cell(rowIdx, 3).Value = det.Unidad ?? string.Empty;
                        ws.Cell(rowIdx, 4).Value = det.Descripcion ?? string.Empty;
                        ws.Cell(rowIdx, 5).Value = det.NoPartida ?? string.Empty;
                        ws.Cell(rowIdx, 6).Value = det.NoEquivalente ?? string.Empty;
                        ws.Cell(rowIdx, 7).Value = det.Catalogo ?? string.Empty;
                        ws.Cell(rowIdx, 8).Value = det.Pagina ?? string.Empty;

                        for (int c = 1; c <= headers.Length; c++)
                        {
                            ws.Cell(rowIdx, c).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell(rowIdx, c).Style.Border.BottomBorderColor = XLColor.LightGray;
                        }
                        rowIdx++;
                    }

                    int filaObservaciones = 60;
                    int filaObservacionesBaja = 61;
                    ws.Cell(filaObservaciones, 1).Value = "Observaciones:";
                    ws.Cell(filaObservaciones, 1).Style.Font.Bold = true;

                    var rangoObservaciones = ws.Range(filaObservacionesBaja, 1, filaObservacionesBaja + 2, 8);
                    rangoObservaciones.Merge();
                    rangoObservaciones.FirstCell().Value = Observaciones ?? string.Empty;
                    rangoObservaciones.Style.Alignment.WrapText = true;
                    rangoObservaciones.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                    int filaFirmas = filaObservaciones + 8;
                    ws.Cell(filaFirmas, 2).Value = "___________________________________";
                    ws.Cell(filaFirmas, 5).Value = "___________________________________";

                    filaFirmas++;
                    ws.Cell(filaFirmas, 2).Value = $"Firma Solicitante: {App.Session?.NombreCompleto}";
                    ws.Cell(filaFirmas, 5).Value = $"Firma Autorizador: {FirmaAutorizante}";
                    ws.Cell(filaFirmas, 2).Style.Font.Bold = true;
                    ws.Cell(filaFirmas, 5).Style.Font.Bold = true;

                    if (!string.IsNullOrWhiteSpace(App.Session?.FirmaPath))
                    {
                        try
                        {
                            string urlFirmaWeb = $"http://enlaceferroviario.com{App.Session.FirmaPath}";

                            using (var webClient = new WebClient())
                            {
                                webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

                                byte[] imageBytes = webClient.DownloadData(urlFirmaWeb);

                                if (imageBytes != null && imageBytes.Length > 0)
                                {
                                    using (var ms = new MemoryStream(imageBytes))
                                    {
                                        var image = ws.AddPicture(ms);
                                        image.Name = "FirmaSolicitante";
                                        image.MoveTo(ws.Cell(filaFirmas - 3, 2));
                                        image.WithSize(140, 50);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al descargar la firma: " + ex.Message);
                        }
                    }

                    ws.PageSetup.PrintAreas.Add("A1:H80");
                    ws.PageSetup.FitToPages(1, 0);
                    ws.PageSetup.Footer.Center.AddText("Constructora Gallo Meda S.A. de C.V. Detroit 16. Col. Ferrocarril. Guadalajara. Jalisco. Mexico. C.P. 44440 Tel. (3339423080)");

                    workbook.SaveAs(rutaTemporal);
                }

                bool exito = _adquisicionService.NuevaRequisicion(
                    idSolicitante: App.Session?.IdUsuario ?? 0,
                    idAutorizante: UsuarioAutorizanteSeleccionado?.IdUsuario ?? 0,
                    idAtencion: UsuarioAtencionSeleccionado?.IdUsuario ?? 0,
                    idUbicacion: idUbicacionReal,
                    fechaRequisicion: DateOnly.FromDateTime(FechaActual),
                    tipoRequisicion: TipoRequiSeleccionada ?? "REQUISICION GENERICA",
                    empresa: EmpresaSeleccionada ?? "CGM",
                    estatus: "EN ESPERA DE SER AUTORIAZADA",
                    rutaArchivoLocal: rutaTemporal,
                    nombreArchivoRemoto: nombreArchivoRemoto,
                    idEconomico: EconomicoSeleccionado?.IdEconomico,
                    gradoPrioridad: GradoPrioridadSeleccionado
                );

                if (exito)
                {
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel Files|*.xlsx",
                        FileName = nombreArchivoRemoto
                    };

                    MessageBox.Show("Requisición guardada en el servidor FTP y en la base de datos exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    RequisicionRegistrada?.Invoke(this, EventArgs.Empty);
                    _onRegistroRequisicionCompletado?.Invoke();
                }
            }
            catch (Exception ex)
            {
                string mensajeReal = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show($"Ocurrió un error al procesar la requisición: {mensajeReal}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CargarCatalogos()
        {
            try
            {
                var ubicacionesDb = _contexto.CatalogoUbicacionesProyectos
                    .Where(e => !string.IsNullOrEmpty(e.Siglas)) // Filtra nulos, vacíos y espacios si usas IsNullOrWhiteSpace
                    .AsNoTracking()
                    .ToList();

                Ubicaciones.Clear();
                foreach (var item in ubicacionesDb) { Ubicaciones.Add(item); }

                var economicosDb = _contexto.CatalogoEconomicos.AsNoTracking().ToList();
                Economicos.Clear();
                foreach (var item in economicosDb) { Economicos.Add(item); }

                var marcasDb = _contexto.Set<CatalogoMarca>().AsNoTracking().ToList();
                ListaMarcas.Clear();
                foreach (var marca in marcasDb) { ListaMarcas.Add(marca); }

                var combustiblesDb = _contexto.Set<CatalogoTiposCombustible>().AsNoTracking().ToList();
                ListaMotores.Clear();
                foreach (var combustible in combustiblesDb) { ListaMotores.Add(combustible); }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar catálogos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _ejecutar;
        private readonly Func<object?, bool>? _puedeEjecutar;
        private Action abrirArchivoExcel;

        public RelayCommand(Action abrirArchivoExcel)
        {
            this.abrirArchivoExcel = abrirArchivoExcel;
        }

        public RelayCommand(Action<object?> ejecutar, Func<object?, bool>? puedeEjecutar = null)
        {
            _ejecutar = ejecutar ?? throw new ArgumentNullException(nameof(ejecutar));
            _puedeEjecutar = puedeEjecutar;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _puedeEjecutar?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => _ejecutar(parameter);
    }
}