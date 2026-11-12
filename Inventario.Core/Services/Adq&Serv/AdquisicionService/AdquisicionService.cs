using ClosedXML.Excel;
using Inventario.Core.DTOs.Requisicion;
using Inventario.Core.Services; // Asegúrate de incluir el espacio de nombres de tu EmailService
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace Inventario.Core.Services.Adq_Serv.AdquisicionService
{
    public class AdquisicionService
    {
        private readonly InventarioContext _context;
        private readonly EmailService _emailService;
        private DateTime _fechaActual = DateTime.Now;
        private readonly string _hostFtp = "ftp://170.10.162.13/";
        private readonly string _usuarioFtp = "dbibiano@enlaceferroviario.com";
        private readonly string _contrasenaFtp = "drbr11122003DRBR.";
        private readonly string _directorioRemoto = "servidor/ArchivosEconomicos/Requisiciones";

        public AdquisicionService(InventarioContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<string> AutorizarYDescargarArchivoAsync(string idRequisicion, string nombreArchivoRemoto, string firmaPath)
        {
            try
            {
                if (string.IsNullOrEmpty(nombreArchivoRemoto))
                {
                    throw new Exception("El nombre del archivo remoto es nulo o está vacío.");
                }

                string directorioLimpio = _directorioRemoto.TrimEnd('/');
                string archivoLimpio = nombreArchivoRemoto.TrimStart('/');
                string urlRemota = $"{_hostFtp.TrimEnd('/')}/{directorioLimpio}/{archivoLimpio}";
                string soloNombreArchivo = Path.GetFileName(nombreArchivoRemoto);
                string rutaLocalTemporal = Path.Combine(Path.GetTempPath(), soloNombreArchivo);
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_usuarioFtp, _contrasenaFtp);
                    client.DownloadFile(urlRemota, rutaLocalTemporal);
                }

                using (var workbook = new XLWorkbook(rutaLocalTemporal))
                {
                    var ws = workbook.Worksheet(1);

                    if (!string.IsNullOrWhiteSpace(firmaPath))
                    {
                        string urlFirmaWeb = $"http://enlaceferroviario.com{firmaPath}";

                        using (var webClient = new WebClient())
                        {
                            webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                            byte[] imageBytes = webClient.DownloadData(urlFirmaWeb);

                            if (imageBytes != null && imageBytes.Length > 0)
                            {
                                using (var ms = new MemoryStream(imageBytes))
                                {
                                    var image = ws.AddPicture(ms);
                                    image.Name = "FirmaAutorizante";
                                    image.MoveTo(ws.Cell(66, 5));
                                    image.WithSize(140, 50);
                                }
                            }
                        }
                    }
                    ws.Protect("");
                    workbook.Save();
                }



                SubirArchivoPorFtp(rutaLocalTemporal, nombreArchivoRemoto);
                var requisicionDb = _context.Requisiciones
                    .Include(r => r.IdAtencionNavigation)
                    .FirstOrDefault(r => r.IdRequisicion == idRequisicion);

                if (requisicionDb != null)
                {
                    // 2. Actualizar el campo de estatus (ajusta "Estatus" por el nombre real de tu propiedad)
                    requisicionDb.Estatus = "EN ESPERA DE SER ATENDIDA";

                    // 3. Guardar los cambios en la base de datos
                    await _context.SaveChangesAsync();
                }

                return rutaLocalTemporal;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al autorizar el archivo: {ex.Message}", ex);
            }
        }


        public async Task<string> NoAutorizarAsync(string idRequisicion)
        {
            var requisicionDb = await _context.Requisiciones
                        .Include(r => r.IdAtencionNavigation)
                        .FirstOrDefaultAsync(r => r.IdRequisicion == idRequisicion);

            if (requisicionDb != null)
            {
                // Actualizar el campo de estatus
                requisicionDb.Estatus = "NO AUTORIZADA";

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
                return "No se autorizo la requisicion";
            }

            return "No se encontró la requisición";
        }

        private void SubirArchivoPorFtp(string rutaLocal, string nombreRemoto)
        {
            string directorioLimpio = _directorioRemoto.TrimEnd('/');
            string archivoLimpio = nombreRemoto.TrimStart('/');
            string urlDestino = $"{_hostFtp.TrimEnd('/')}/{directorioLimpio}/{archivoLimpio}";
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(urlDestino);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_usuarioFtp, _contrasenaFtp);
            request.UseBinary = true;
            request.UsePassive = true;
            byte[] fileContents = File.ReadAllBytes(rutaLocal);
            request.ContentLength = fileContents.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }
        }

        public bool NuevaRequisicion(int idSolicitante, int idAutorizante, int idAtencion,
                                   int idUbicacion, DateOnly fechaRequisicion, string tipoRequisicion,
                                   string empresa, string estatus, string rutaArchivoLocal, 
                                   string nombreArchivoRemoto, string idEconomico, string gradoPrioridad)
        {
            try
            {
                if (!string.IsNullOrEmpty(rutaArchivoLocal) && File.Exists(rutaArchivoLocal))
                {
                    SubirArchivoPorFtp(rutaArchivoLocal, nombreArchivoRemoto);
                }
                else
                {
                    throw new FileNotFoundException("El archivo local a subir no existe o la ruta está vacía.");
                }
                int ultimoConsecutivo = _context.Requisiciones
                    .Where(r => r.Empresa == empresa && r.IdUbicacion == idUbicacion)
                    .Select(r => (int?)r.Consecutivo)
                    .Max() ?? 0;
                int siguienteConsecutivo = ultimoConsecutivo + 1;
                string idRequisicionGenerado = $"{empresa} - RC - {siguienteConsecutivo:D3} - {idUbicacion} - {_fechaActual:yyyy}";
                var nuevaRequisicion = new Requisicione
                {
                    IdRequisicion = idRequisicionGenerado,
                    IdUbicacion = idUbicacion,
                    Consecutivo = siguienteConsecutivo,
                    FechaRequisicion = fechaRequisicion,
                    TipoRequisicion = tipoRequisicion,
                    Empresa = empresa,
                    Estatus = estatus,
                    ArchivoReq = nombreArchivoRemoto,
                    IdSolicitante = idSolicitante,
                    IdAutorizante = idAutorizante,
                    IdAtencion = idAtencion,
                    IdEconomico = idEconomico,
                    GradoPrioridad = gradoPrioridad
                };
                _context.Requisiciones.Add(nuevaRequisicion);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al procesar la nueva requisición: {ex.Message}");
            }
        }

        public async Task<bool> LiquidarRequisicionAsync(string idRequisicion)
        {
            try
            {
                var requisicionDb = await _context.Requisiciones
                    .FirstOrDefaultAsync(r => r.IdRequisicion == idRequisicion);

                if (requisicionDb != null)
                {
                    requisicionDb.Estatus = "LIQUIDADA";
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al liquidar la requisición: {ex.Message}", ex);
            }
        }

        public async Task<bool> LiquidarProcesoRequisicionAsync(string idRequisicion)
        {
            try
            {
                var requisicionDb = await _context.Requisiciones
                    .FirstOrDefaultAsync(r => r.IdRequisicion == idRequisicion);

                if (requisicionDb != null)
                {
                    requisicionDb.Estatus = "EN PROCESO DE SER LIQUIDADA";
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al liquidar la requisición: {ex.Message}", ex);
            }
        }

        public async Task<List<RequisicionListDto>> ObtenerRequisicionesAsync(int idUsuario)
        {
            return await _context.Requisiciones
                .Where(r => r.IdAutorizante == idUsuario && r.Estatus == "EN ESPERA DE SER AUTORIZADA")
                .Include(r => r.IdSolicitanteNavigation)
                .Include(r => r.IdUbicacionNavigation)
                .Include(r => r.IdAtencionNavigation)
                .Include(r => r.IdAutorizanteNavigation)
                .OrderByDescending(r => r.FechaRequisicion)
                .Select(r => new RequisicionListDto
                {
                    IdRequisicion = r.IdRequisicion,
                    FechaRequisicion = (DateOnly)r.FechaRequisicion,
                    Empresa = r.Empresa,
                    IdEconomico = r.IdEconomico,
                    Estatus = r.Estatus,
                    TipoRequisicion = r.TipoRequisicion,
                    GradoPrioridad = r.GradoPrioridad,
                    Ubicacion = r.IdUbicacionNavigation != null ? r.IdUbicacionNavigation.Ubicacion : "N/A",
                    Solicitante = r.IdSolicitanteNavigation != null ? r.IdSolicitanteNavigation.NombreCompleto : "N/A",
                    Autorizante = r.IdAutorizanteNavigation != null ? r.IdAutorizanteNavigation.NombreCompleto : "N/A",
                    Atencion = r.IdAtencionNavigation != null ? r.IdAtencionNavigation.NombreCompleto : "N/A",
                    CorreoAtencion = r.IdAtencionNavigation != null ? r.IdAtencionNavigation.Correoe : string.Empty,
                    CorreoSolicitante = r.IdSolicitanteNavigation != null ? r.IdSolicitanteNavigation.Correoe : string.Empty,
                    ArchivoReq = r.ArchivoReq
                })
                .ToListAsync();
        }

        public async Task<List<RequisicionListDto>> ObtenerTodasRequisicionesAsync()
        {
            return await _context.Requisiciones
                .Where(r => r.Estatus != "FANTASMA")
                .Include(r => r.IdSolicitanteNavigation)
                .Include(r => r.IdUbicacionNavigation)
                .Include(r => r.IdAtencionNavigation)
                .Include(r => r.IdAutorizanteNavigation)
                .OrderByDescending(r => r.FechaRequisicion)
                .Select(r => new RequisicionListDto
                {
                    IdRequisicion = r.IdRequisicion,
                    IdEconomico = r.IdEconomico,
                    FechaRequisicion = (DateOnly)r.FechaRequisicion,
                    Empresa = r.Empresa,
                    GradoPrioridad = r.GradoPrioridad,
                    Estatus = r.Estatus,
                    Ubicacion = r.IdUbicacionNavigation != null ? r.IdUbicacionNavigation.Ubicacion: "N/A",
                    Solicitante = r.IdSolicitanteNavigation != null ? r.IdSolicitanteNavigation.NombreCompleto : "N/A",
                    Autorizante = r.IdAutorizanteNavigation != null ? r.IdAutorizanteNavigation.NombreCompleto : "N/A",
                    Atencion = r.IdAtencionNavigation != null ? r.IdAtencionNavigation.NombreCompleto : "N/A",                   
                    ArchivoReq = r.ArchivoReq
                })
                .ToListAsync();
        }

        public async Task<List<Requisicione>> ObtenerRequisicionesPorIdsAsync(string[] ids)
        {
            return await _context.Requisiciones
                .Where(r => ids.Contains(r.IdRequisicion))
                .Include(r => r.IdSolicitanteNavigation)
                .Include(r => r.IdUbicacionNavigation)
                .ToListAsync();
        }


        public void DescargarYAbrirArchivoExcel(string nombreArchivoRemoto)
        {
            try
            {
                if (string.IsNullOrEmpty(nombreArchivoRemoto))
                {
                    throw new ArgumentException("El nombre del archivo remoto es nulo o está vacío.", nameof(nombreArchivoRemoto));
                }

                string archivoLimpio = nombreArchivoRemoto.Replace("\\", "/").TrimStart('/');
                string directorioLimpio = _directorioRemoto.TrimEnd('/');
                string urlRemota = $"{_hostFtp.TrimEnd('/')}/{directorioLimpio}/{archivoLimpio}";
                string soloNombreArchivo = Path.GetFileName(nombreArchivoRemoto);
                string rutaLocalTemporal = Path.Combine(Path.GetTempPath(), soloNombreArchivo);
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_usuarioFtp, _contrasenaFtp);
                    client.DownloadFile(urlRemota, rutaLocalTemporal);
                }
                if (File.Exists(rutaLocalTemporal))
                {
                    Process.Start(new ProcessStartInfo(rutaLocalTemporal) { UseShellExecute = true });
                }
                else
                {
                    throw new FileNotFoundException("No se pudo descargar el archivo temporal desde el FTP.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en DescargarYAbrirArchivoExcel: {ex.Message}", ex);
            }
        }
    }
}