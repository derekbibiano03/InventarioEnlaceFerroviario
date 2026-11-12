using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Inventario.Core.Services.Economicos
{
    public class GestorArchivosService
    {
        private readonly InventarioContext _context;
        private readonly string _hostFtp = "ftp://170.10.162.13/";
        private readonly string _usuarioFtp = "dbibiano@enlaceferroviario.com";
        private readonly string _contrasenaFtp = "drbr11122003DRBR.";
        private readonly string _directorioRemoto = "servidor/ArchivosEconomicos/";
        private readonly string _directorioRemotoResguardos = "servidor/ArchivosEconomicos/Resguardos/";
        private readonly string _directorioRemotoMovimientos = "servidor/ArchivosEconomicos/MovimientosEconomicos/";
        public GestorArchivosService(InventarioContext context)
        {
            _context = context;
            string rutaTemporalLocal = Path.Combine(Path.GetTempPath(), "ArchivosEconomicosTemp");
            if (!Directory.Exists(rutaTemporalLocal))
            {
                Directory.CreateDirectory(rutaTemporalLocal);
            }
        }

        private void SubirArchivoPorFtp(string rutaLocal, string nombreRemoto)
        {
            string urlDestino = $"{_hostFtp}{_directorioRemoto}{nombreRemoto}";
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

        private void SubirArchivoPorFtpMovimientos(string rutaLocal, string nombreRemoto)
        {
            string urlDestino = $"{_hostFtp}{_directorioRemotoMovimientos}{nombreRemoto}";
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

        private void SubirArchivoPorFtpResguardos(string rutaLocal, string nombreRemoto)
        {
            string urlDestino = $"{_hostFtp}{_directorioRemotoResguardos}{nombreRemoto}";
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

        public List<(CatalogoArchivo Archivo, int IdServicio)> RegistrarArchivosServicios(List<string> rutasOriginales, int idServicio)
        {
            var registrosLog = new List<(CatalogoArchivo Archivo, int IdServicio)>();
            if (rutasOriginales == null || !rutasOriginales.Any())
            {
                return registrosLog;
            }

            foreach (string rutaOriginal in rutasOriginales)
            {
                if (!File.Exists(rutaOriginal))
                {
                    throw new FileNotFoundException($"El archivo no fue encontrado en la ruta especificada: {rutaOriginal}");
                }

                string nombreOriginal = Path.GetFileName(rutaOriginal);
                string extension = Path.GetExtension(rutaOriginal);
                string nombreFisicoUnico = Guid.NewGuid().ToString() + extension;
                SubirArchivoPorFtp(rutaOriginal, nombreFisicoUnico);

                var catalogoArchivo = new CatalogoArchivo
                {
                    Archivo = nombreFisicoUnico,
                    NombreArchivo = nombreOriginal,
                    FechaSubida = DateTime.UtcNow
                };

                _context.CatalogoArchivos.Add(catalogoArchivo);
                _context.SaveChanges();

                var relacionServicio = new ServicioArchivo
                {
                    IdServicio = idServicio,
                    IdArchivo = catalogoArchivo.IdArchivo
                };

                _context.ServicioArchivos.Add(relacionServicio);
                _context.SaveChanges();

                registrosLog.Add((catalogoArchivo, idServicio));
            }

            return registrosLog;
        }

        public List<(CatalogoArchivo Archivo, int IdResguardo)> RegistrarArchivosResguardos(List<string> rutasOriginales, int idResguardo)
        {
            var registrosLog = new List<(CatalogoArchivo Archivo, int IdResguardo)>();
            if (rutasOriginales == null || !rutasOriginales.Any())
            {
                return registrosLog;
            }

            foreach (string rutaOriginal in rutasOriginales)
            {
                if (!File.Exists(rutaOriginal))
                {
                    throw new FileNotFoundException($"El archivo no fue encontrado en la ruta especificada: {rutaOriginal}");
                }

                string nombreOriginal = Path.GetFileName(rutaOriginal);
                string extension = Path.GetExtension(rutaOriginal);
                string nombreFisicoUnico = Guid.NewGuid().ToString() + extension;
                SubirArchivoPorFtpResguardos(rutaOriginal, nombreFisicoUnico);

                var catalogoArchivo = new CatalogoArchivo
                {
                    Archivo = nombreFisicoUnico,
                    NombreArchivo = nombreOriginal,
                    FechaSubida = DateTime.UtcNow
                };

                _context.CatalogoArchivos.Add(catalogoArchivo);
                _context.SaveChanges();

                var relacionResguardo = new ResguardosArchivo
                {
                    IdResguardo = idResguardo,
                    IdArchivo = catalogoArchivo.IdArchivo
                };

                _context.ResguardosArchivos.Add(relacionResguardo);
                _context.SaveChanges();

                registrosLog.Add((catalogoArchivo, idResguardo));
            }

            return registrosLog;
        }

        public List<(CatalogoArchivo Archivo, int IdMovimiento)> RegistrarArchivosMovimientos(List<string> rutasOriginales, int idMovimiento)
        {
            var registrosLog = new List<(CatalogoArchivo Archivo, int IdMovimiento)>();
            if (rutasOriginales == null || !rutasOriginales.Any())
            {
                return registrosLog;
            }

            var movimiento = _context.CatalogoMovimientosEconomicos
                .Include(m => m.MovimientosArchivos)
                .FirstOrDefault(m => m.IdMovimiento == idMovimiento);

            if (movimiento == null)
            {
                throw new Exception($"No se encontró el movimiento con ID {idMovimiento}");
            }

            foreach (string rutaOriginal in rutasOriginales)
            {
                if (!File.Exists(rutaOriginal))
                {
                    throw new FileNotFoundException($"El archivo no fue encontrado en la ruta especificada: {rutaOriginal}");
                }

                string nombreOriginal = Path.GetFileName(rutaOriginal);
                string extension = Path.GetExtension(rutaOriginal);
                string nombreFisicoUnico = Guid.NewGuid().ToString() + extension;

                SubirArchivoPorFtpMovimientos(rutaOriginal, nombreFisicoUnico);

                // 1. Creamos y guardamos el archivo en el catálogo principal
                var catalogoArchivo = new CatalogoArchivo
                {
                    Archivo = nombreFisicoUnico,
                    NombreArchivo = nombreOriginal,
                    FechaSubida = DateTime.UtcNow
                };

                _context.CatalogoArchivos.Add(catalogoArchivo);
                _context.SaveChanges();

                // 2. Creamos la relación intermedia correcta usando el modelo de la tabla puente
                var relacionMovimiento = new MovimientosArchivo
                {
                    IdMovimiento = idMovimiento,
                    IdArchivo = catalogoArchivo.IdArchivo
                };

                _context.MovimientosArchivos.Add(relacionMovimiento);
                _context.SaveChanges();

                registrosLog.Add((catalogoArchivo, idMovimiento));
            }

            return registrosLog;
        }


        public List<(CatalogoArchivo Archivo, string IdEconomico)> RegistrarArchivosEconomicos(List<string> rutasOriginales, List<string> idsEconomicos)
        {
            var registrosLog = new List<(CatalogoArchivo Archivo, string IdEconomico)>();
            if (rutasOriginales == null || !rutasOriginales.Any() || idsEconomicos == null || !idsEconomicos.Any())
            {
                return registrosLog;
            }

            foreach (string rutaOriginal in rutasOriginales)
            {
                if (!File.Exists(rutaOriginal))
                {
                    throw new FileNotFoundException($"El archivo físico no existe: {rutaOriginal}");
                }

                string nombreOriginal = Path.GetFileName(rutaOriginal);
                string extension = Path.GetExtension(rutaOriginal);
                string nombreFisicoUnico = Guid.NewGuid().ToString() + extension;
                SubirArchivoPorFtp(rutaOriginal, nombreFisicoUnico);
                CatalogoArchivo nuevoArchivo = new CatalogoArchivo
                {
                    Archivo = nombreFisicoUnico,
                    NombreArchivo = nombreOriginal,
                    FechaSubida = DateTime.UtcNow
                };

                _context.CatalogoArchivos.Add(nuevoArchivo);

                foreach (string idEconomico in idsEconomicos)
                {
                    EconomicosArchivo nuevaRelacion = new EconomicosArchivo
                    {
                        IdEconomico = idEconomico,
                        IdArchivoNavigation = nuevoArchivo
                    };

                    _context.EconomicosArchivos.Add(nuevaRelacion);
                    registrosLog.Add((nuevoArchivo, idEconomico));
                }
            }
            return registrosLog;
        }

        public string GuardarArchivo(string rutaOriginal)
        {
            if (!File.Exists(rutaOriginal))
            {
                throw new FileNotFoundException("El archivo de origen no existe en la ruta especificada.");
            }

            string extension = Path.GetExtension(rutaOriginal);
            string nuevoNombre = Guid.NewGuid().ToString() + extension;
            SubirArchivoPorFtp(rutaOriginal, nuevoNombre);
            return nuevoNombre;
        }

        public List<(CatalogoArchivo Archivo, int IdServicio)> RegistrarArchivosServiciosConGuid(List<string> rutasArchivos, int idServicio)
        {
            var registrosLog = new List<(CatalogoArchivo Archivo, int IdServicio)>();
            if (rutasArchivos == null || !rutasArchivos.Any())
            {
                return registrosLog;
            }

            foreach (string rutaLocal in rutasArchivos)
            {
                if (!File.Exists(rutaLocal))
                {
                    throw new FileNotFoundException($"El archivo no fue encontrado en la ruta especificada: {rutaLocal}");
                }

                string nombreOriginal = Path.GetFileName(rutaLocal);
                string extension = Path.GetExtension(rutaLocal);
                string nombreFisicoUnico = Guid.NewGuid().ToString() + extension;
                SubirArchivoPorFtp(rutaLocal, nombreFisicoUnico);
                var catalogoArchivo = new CatalogoArchivo
                {
                    Archivo = nombreFisicoUnico,
                    NombreArchivo = nombreOriginal,
                    FechaSubida = DateTime.UtcNow
                };

                _context.CatalogoArchivos.Add(catalogoArchivo);
                _context.SaveChanges();

                var relacionServicio = new ServicioArchivo
                {
                    IdServicio = idServicio,
                    IdArchivo = catalogoArchivo.IdArchivo
                };

                _context.ServicioArchivos.Add(relacionServicio);
                _context.SaveChanges();

                registrosLog.Add((catalogoArchivo, idServicio));
            }

            return registrosLog;
        }

        public string ObtenerRutaAbsoluta(string nombreArchivoBD)
        {
            if (string.IsNullOrWhiteSpace(nombreArchivoBD))
                return string.Empty;

            string nombreLimpio = Path.GetFileName(nombreArchivoBD);
            string rutaLocalTemporal = Path.Combine(Path.GetTempPath(), "ArchivosEconomicosTemp", nombreLimpio);

            if (!File.Exists(rutaLocalTemporal))
            {
                string baseHost = _hostFtp.EndsWith("/") ? _hostFtp : _hostFtp + "/";
                bool descargado = false;

                // AGREGA _directorioRemotoMovimientos AQUÍ PARA QUE LO BUSQUE CORRECTAMENTE
                string[] directoriosAProbar = { _directorioRemotoMovimientos, _directorioRemotoResguardos, _directorioRemoto };

                foreach (var dir in directoriosAProbar)
                {
                    try
                    {
                        string directorio = dir.Trim('/') + "/";
                        string urlOrigen = $"{baseHost}{directorio}{nombreLimpio}";

                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(urlOrigen);
                        request.Method = WebRequestMethods.Ftp.DownloadFile;
                        request.Credentials = new NetworkCredential(_usuarioFtp, _contrasenaFtp);
                        request.UseBinary = true;
                        request.UsePassive = true;

                        using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                        using (Stream responseStream = response.GetResponseStream())
                        using (FileStream fileStream = File.Create(rutaLocalTemporal))
                        {
                            responseStream.CopyTo(fileStream);
                        }

                        descargado = true;
                        break;
                    }
                    catch
                    {
                        // Ignora y prueba con el siguiente directorio
                    }
                }

                if (!descargado)
                {
                    throw new FileNotFoundException($"No se pudo encontrar el archivo {nombreLimpio} en ninguna de las rutas FTP del servidor.");
                }
            }

            return rutaLocalTemporal;
        }

        public void EliminarArchivosServicio(List<int> idsArchivos)
        {
            if (idsArchivos == null || idsArchivos.Count == 0) return;

            var relaciones = _context.ServicioArchivos
                                      .Where(sa => idsArchivos.Contains(sa.IdArchivoNavigation.IdArchivo))
                                      .ToList();

            var archivos = _context.CatalogoArchivos
                                   .Where(a => idsArchivos.Contains(a.IdArchivo))
                                   .ToList();

            foreach (var archivo in archivos)
            {
                if (!string.IsNullOrEmpty(archivo.Archivo))
                {
                    string rutaAbsoluta = ObtenerRutaAbsoluta(archivo.Archivo);
                    if (System.IO.File.Exists(rutaAbsoluta))
                    {
                        System.IO.File.Delete(rutaAbsoluta);
                    }
                }
            }

            _context.ServicioArchivos.RemoveRange(relaciones);
            _context.CatalogoArchivos.RemoveRange(archivos);

            _context.SaveChanges();
        }

        public void EliminarArchivosMovimientos(List<int> idsArchivos)
        {
            if (idsArchivos == null || idsArchivos.Count == 0) return;

            var relaciones = _context.MovimientosArchivos
                                      .Where(sa => idsArchivos.Contains(sa.IdArchivoNavigation.IdArchivo))
                                      .ToList();

            var archivos = _context.CatalogoArchivos
                                   .Where(a => idsArchivos.Contains(a.IdArchivo))
                                   .ToList();

            foreach (var archivo in archivos)
            {
                if (!string.IsNullOrEmpty(archivo.Archivo))
                {
                    string rutaAbsoluta = ObtenerRutaAbsoluta(archivo.Archivo);
                    if (System.IO.File.Exists(rutaAbsoluta))
                    {
                        System.IO.File.Delete(rutaAbsoluta);
                    }
                }
            }

            _context.MovimientosArchivos.RemoveRange(relaciones);
            _context.CatalogoArchivos.RemoveRange(archivos);

            _context.SaveChanges();
        }

    }
}