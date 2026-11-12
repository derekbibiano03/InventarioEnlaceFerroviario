using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Inventario.Core.Services.Economicos
{
    public class HistorialServicioService
    {
        private readonly CatalogoUbicacionesProyecto _ubicaciones;
        private readonly InventarioContext _context;
        private readonly LogsService _logsService;
        private readonly string _hostServidor = "170.10.162.13";
        private readonly string _usuarioFtp = "dbibiano@enlaceferroviario.com";
        private readonly string _contrasenaFtp = "drbr11122003DRBR.";

        public HistorialServicioService(InventarioContext context, LogsService logsService)
        {
            _context = context;
            _logsService = logsService;
        }

        public HistorialServicio RegistrarServicio(int idUsuarioOperativo, HistorialServicio dto)
        {
            var economicoReal = _context.CatalogoEconomicos
                .FirstOrDefault(e => e.IdEconomico == dto.NoEconomico); 
            var nuevoRegistro = new HistorialServicio
            {
                NoEconomico = dto.NoEconomico,
                FechaMantenimiento = dto.FechaMantenimiento,
                TipoMantenimiento = dto.TipoMantenimiento,
                Anotaciones = dto.Anotaciones,
                Horaskilometrosreales = dto.Horaskilometrosreales,
                Costos = dto.Costos,
                UbicacionRealizacion = economicoReal?.IdUbicacion, 
                IdUsuario = dto.IdUsuario,
                IdGrupo = economicoReal?.IdGrupo                   
            };

            _context.HistorialServicios.Add(nuevoRegistro);
            _context.SaveChanges();

            int idServicio = nuevoRegistro.IdServicio;
            _logsService.RegistrarServicioEquipo(idUsuarioOperativo, idServicio);

            return nuevoRegistro;
        }

        public void GuardarArchivosRelacionados(List<(CatalogoArchivo archivoCatalogado, ServicioArchivo relacionServicio)> archivosProcesados)
        {
            foreach (var item in archivosProcesados)
            {
                _context.CatalogoArchivos.Add(item.archivoCatalogado);
                _context.ServicioArchivos.Add(item.relacionServicio);
            }

            _context.SaveChanges();
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

        public List<HistorialServicio> ObtenerHistorial()
        {
            return _context.HistorialServicios
                .Include(s => s.NoEconomicoNavigation)
                .Include(h => h.IdUsuarioNavigation)      // <-- Carga el usuario
                .Include(h => h.IdGrupoNavigation)
                .Include(s => s.ServicioArchivos)
                    .ThenInclude(sa => sa.IdArchivoNavigation)
                .Include(s => s.UbicacionRealizacionNavigation)
                .ToList();
        }

        public void ModificarServicio(HistorialServicio dto, List<(CatalogoArchivo archivoCatalogado, ServicioArchivo relacionServicio)> nuevosArchivos, List<int> idsArchivosAEliminar)
        {
            var servicioExistente = _context.HistorialServicios
                .Include(s => s.ServicioArchivos)
                .FirstOrDefault(s => s.IdServicio == dto.IdServicio);

            if (servicioExistente == null)
            {
                throw new Exception($"El servicio con ID {dto.IdServicio} no existe.");
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                servicioExistente.NoEconomico = dto.NoEconomico;
                servicioExistente.FechaMantenimiento = dto.FechaMantenimiento;
                servicioExistente.TipoMantenimiento = dto.TipoMantenimiento;
                servicioExistente.Anotaciones = dto.Anotaciones;
                servicioExistente.Horaskilometrosreales = dto.Horaskilometrosreales;

                if (idsArchivosAEliminar != null && idsArchivosAEliminar.Any())
                {
                    var relacionesAEliminar = servicioExistente.ServicioArchivos
                        .Where(sa => sa.IdArchivo.HasValue && idsArchivosAEliminar.Contains(sa.IdArchivo.Value))
                        .ToList();

                    foreach (var relacion in relacionesAEliminar)
                    {
                        _context.ServicioArchivos.Remove(relacion);

                        if (relacion.IdArchivo.HasValue)
                        {
                            var archivoCatalogo = _context.CatalogoArchivos.Find(relacion.IdArchivo.Value);
                            if (archivoCatalogo != null)
                            {
                                _context.CatalogoArchivos.Remove(archivoCatalogo);
                            }
                        }
                    }
                }

                if (nuevosArchivos != null && nuevosArchivos.Any())
                {
                    foreach (var item in nuevosArchivos)
                    {
                        _context.CatalogoArchivos.Add(item.archivoCatalogado);
                        _context.SaveChanges();

                        item.relacionServicio.IdServicio = servicioExistente.IdServicio;
                        item.relacionServicio.IdArchivo = item.archivoCatalogado.IdArchivo;

                        _context.ServicioArchivos.Add(item.relacionServicio);
                    }
                }

                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public void EliminarServicio(int idServicio)
        {
            var servicio = _context.HistorialServicios
                                   .Include(s => s.ServicioArchivos)
                                   .FirstOrDefault(s => s.IdServicio == idServicio);

            if (servicio != null)
            {
                if (servicio.ServicioArchivos != null && servicio.ServicioArchivos.Any())
                {
                    foreach (var relacion in servicio.ServicioArchivos.ToList())
                    {
                        _context.ServicioArchivos.Remove(relacion);

                        if (relacion.IdArchivo.HasValue)
                        {
                            var archivoCatalogo = _context.CatalogoArchivos.Find(relacion.IdArchivo.Value);
                            if (archivoCatalogo != null)
                            {
                                _context.CatalogoArchivos.Remove(archivoCatalogo);
                            }
                        }
                    }
                }

                _context.HistorialServicios.Remove(servicio);
                _context.SaveChanges();
            }
        }

        public void ActualizarServicio(HistorialServicio servicioEditado)
        {
            var servicioExistente = _context.HistorialServicios
                                            .FirstOrDefault(s => s.IdServicio == servicioEditado.IdServicio);

            if (servicioExistente == null)
            {
                throw new Exception($"No se encontró el registro de servicio con ID {servicioEditado.IdServicio}.");
            }

            servicioExistente.FechaMantenimiento = servicioEditado.FechaMantenimiento;
            servicioExistente.TipoMantenimiento = servicioEditado.TipoMantenimiento;
            servicioExistente.Anotaciones = servicioEditado.Anotaciones;
            servicioExistente.Horaskilometrosreales = servicioEditado.Horaskilometrosreales;

            _context.SaveChanges();
        }
    }
}