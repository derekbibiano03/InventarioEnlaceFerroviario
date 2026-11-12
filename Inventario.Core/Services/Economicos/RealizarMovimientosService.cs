using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Inventario.Core.Services.Economicos
{
    public class RealizarMovimientosService
    {
        private readonly InventarioContext _context;
        private readonly LogsService _logsService;

        public RealizarMovimientosService(InventarioContext context, LogsService logsService)
        {
            _context = context;
            _logsService = logsService;
        }

        private readonly string _hostServidor = "170.10.162.13";
        private readonly string _usuarioFtp = "dbibiano@enlaceferroviario.com";
        private readonly string _contrasenaFtp = "drbr11122003DRBR.";

        public CatalogoMovimientosEconomico RegistrarMovimiento(int idUsuarioOperativo, CatalogoMovimientosEconomico dto)
        {
            var economicoReal = _context.CatalogoEconomicos
                .FirstOrDefault(e => e.IdEconomico == dto.IdEconomico);
            var nuevoMovimiento = new CatalogoMovimientosEconomico
            {
                IdEconomico = dto.IdEconomico,
                FechaMovimiento = dto.FechaMovimiento,
                FechaLlegada = dto.FechaLlegada,
                IdUbicacionLlegada = dto.IdUbicacionLlegada,
                IdUbicacionSalida = dto.IdUbicacionSalida,
                Anotaciones = dto.Anotaciones,
                IdUsuario = dto.IdUsuario
            };

            var actualizarUbicacion = _context.CatalogoEconomicos
                                            .FirstOrDefault(s => s.IdEconomico == nuevoMovimiento.IdEconomico);
            actualizarUbicacion.IdUbicacion = nuevoMovimiento.IdUbicacionLlegada;

            _context.CatalogoMovimientosEconomicos.Add(nuevoMovimiento);
            _context.SaveChanges();

            int idMovimiento = nuevoMovimiento.IdMovimiento;
            _logsService.RegistrarMovimientoEquipo(idUsuarioOperativo, idMovimiento);

            return nuevoMovimiento;
        }

        public void GuardarArchivosRelacionados(int idMovimiento, List<CatalogoArchivo> archivosProcesados)
        {
            var movimiento = _context.CatalogoMovimientosEconomicos
                .Include(m => m.MovimientosArchivos)
                .FirstOrDefault(m => m.IdMovimiento == idMovimiento);

            if (movimiento != null && archivosProcesados != null)
            {
                foreach (var archivo in archivosProcesados)
                {
                    // Creamos la relación intermedia correcta apuntando al archivo ya existente
                    var relacionMovimiento = new MovimientosArchivo
                    {
                        IdMovimiento = idMovimiento,
                        IdArchivo = archivo.IdArchivo
                    };

                    _context.MovimientosArchivos.Add(relacionMovimiento);
                }

                _context.SaveChanges();
            }
        }

        public string? ObtenerArchivoTemporalDesdeFtp(string rutaRemotaServidor)
        {
            if (string.IsNullOrWhiteSpace(rutaRemotaServidor))
            {
                return null;
            }
            try
            {
                string rutaLimpia = rutaRemotaServidor.Replace("ftp://", "", StringComparison.OrdinalIgnoreCase).TrimStart('/');

                if (rutaLimpia.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                {
                    rutaLimpia = rutaLimpia.Substring(7);
                }
                else if (rutaLimpia.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    rutaLimpia = rutaLimpia.Substring(8);
                }

                string rutaRelativa = rutaLimpia;
                if (rutaLimpia.StartsWith(_hostServidor, StringComparison.OrdinalIgnoreCase))
                {
                    rutaRelativa = rutaLimpia.Substring(_hostServidor.Length).TrimStart('/');
                }

                string urlFtp = $"ftp://{_hostServidor}/{rutaRelativa}";
                string nombreArchivo = Path.GetFileName(urlFtp);
                string rutaTemporalLocal = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{nombreArchivo}");

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(urlFtp);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(_usuarioFtp, _contrasenaFtp);
                request.UseBinary = true;
                request.UsePassive = true;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (FileStream fileStream = File.Create(rutaTemporalLocal))
                {
                    responseStream.CopyTo(fileStream);
                }

                return rutaTemporalLocal;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el archivo desde el servidor FTP: {ex.Message}", ex);
            }
        }

        public List<CatalogoMovimientosEconomico> ObtenerHistorial()
        {
            return _context.CatalogoMovimientosEconomicos
                .Include(s => s.IdEconomicoNavigation)
                .Include(h => h.IdUbicacionLlegadaNavigation)
                .Include(h => h.IdUbicacionSalidaNavigation)
                .Include(s => s.MovimientosArchivos)
                    .ThenInclude(ma => ma.IdArchivoNavigation) // 👈 ESTO ES LO QUE FALTABA
                .Include(s => s.IdUsuarioNavigation)
                .ToList();
        }

        public void ActualizarMovimiento(CatalogoMovimientosEconomico movimientoModel)
        {
            var movimientoExistente = _context.CatalogoMovimientosEconomicos
                                            .FirstOrDefault(s => s.IdMovimiento == movimientoModel.IdMovimiento);

            if (movimientoExistente == null) // 👈 CORREGIDO: Evaluamos movimientoExistente, no movimientoModel
            {
                throw new Exception($"No se encontró el registro con ID {movimientoModel.IdMovimiento}.");
            }

            // 👈 CORREGIDO: Asignamos al objeto de la base de datos
            movimientoExistente.FechaLlegada = movimientoModel.FechaLlegada;
            movimientoExistente.AnotacionesLlegada = movimientoModel.AnotacionesLlegada; // O AnotacionesLlegada según tu modelo

            _context.SaveChanges();
        }
    }
}