using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Inventario.Core.Services.Logs
{
    public class LogsService
    {
        private readonly InventarioContext _context;

        public LogsService(InventarioContext context)
        {
            _context = context;
        }

        private string ObtenerIpLocal()
        {
            try
            {
                string nombreHost = Dns.GetHostName();

                var ip = Dns.GetHostAddresses(nombreHost)
                    .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                return ip != null ? ip.ToString() : IPAddress.Loopback.ToString();
            }
            catch (Exception)
            {
                return IPAddress.Loopback.ToString();
            }
        }

        // ########### USUARIOS #############

        //REGISTRO DE LOGINS
        public void RegistrarLoginExitoso(int idUsuarioLogueado)
        {
            var nuevoLog = new HistorialLog
            {
                DescripcionLog = $"El usuario con ID '{idUsuarioLogueado}' inició sesión de forma correcta en el sistema.",
                TipoLog = "LOGIN",
                IdUsuario = idUsuarioLogueado,
                IpAddress = ObtenerIpLocal(),
                FechaLog = DateTime.UtcNow
            };

            _context.HistorialLogs.Add(nuevoLog);
            _context.SaveChanges();
        }

        //REGISTRO DE ALTAS DE NUEVOS USUARIOS
        public void RegistrarAltaNuevoUsuarioExitoso(int idUsuarioLogueado, string newuser)
        {
            var nuevoLog = new HistorialLog
            {
                DescripcionLog = $"El usuario con ID '{idUsuarioLogueado}' dio de alta a un nuevo usuario con el id '{newuser}' de manera exitosa en el sistema.",
                TipoLog = "ALTA NUEVO USUARIO",
                IdUsuario = idUsuarioLogueado,
                IpAddress = ObtenerIpLocal(),
                FechaLog = DateTime.UtcNow
            };

            _context.HistorialLogs.Add(nuevoLog);
            _context.SaveChanges();
        }


        // ########### EQUIPOS #############

        //REGISTRAR NUEVO EQUIPO
        public void RegistrarAltaEquipoExitoso(int idUsuarioLogueado, string noEconomico)
        {
            var nuevoLog = new HistorialLog
            {
                DescripcionLog = $"El usuario con ID '{idUsuarioLogueado}' registro el equipo con el id '{noEconomico}' de manera exitosa en el sistema.",
                TipoLog = "ALTA NUEVO EQUIPO",
                IdUsuario = idUsuarioLogueado,
                IpAddress = ObtenerIpLocal(),
                FechaLog = DateTime.UtcNow
            };

            _context.HistorialLogs.Add(nuevoLog);
            _context.SaveChanges();
        }

        //REGISTRO ALTA DE ARCHIVOS PARA ECONOMICOS

        public void RegistrarDocumentoAdjuntoExitoso(int idUsuarioLogueado, string noEconomico, string idEconomico)
        {
            var nuevoLog = new HistorialLog
            {
                DescripcionLog = $"El usuario con ID '{idUsuarioLogueado}' adjunto el documento con el id '{idEconomico}' al economico con el id '{noEconomico}' de manera exitosa en el sistema.",
                TipoLog = "ADJUNTAR NUEVO DOCUMENTO",
                IdUsuario = idUsuarioLogueado,
                IpAddress = ObtenerIpLocal(),
                FechaLog = DateTime.UtcNow
            };

            _context.HistorialLogs.Add(nuevoLog);
            _context.SaveChanges();
        }


        //REGISTRAR MODIFICACION DE INFORMACION DE EQUIPO
        public void RegistrarModificacionEquipo(int idUsuarioLogueado, string noEconomico)
        {
            var nuevoLog = new HistorialLog
            {
                DescripcionLog = $"El usuario con ID '{idUsuarioLogueado}' a modificado la informacion del equipo con el id '{noEconomico}'.",
                TipoLog = "MODIFICACION DE INFORMACION EQUIPO",
                IdUsuario = idUsuarioLogueado,
                IpAddress = ObtenerIpLocal(),
                FechaLog = DateTime.UtcNow
            };

            _context.HistorialLogs.Add(nuevoLog);
            _context.SaveChanges();
        }


        //REGISTRAR NUEVO ARCHIVO DE ECONOMICO
        public void RegistrarArchivoEquipo(int idUsuarioLogueado, string noEconomico, int id_archivo)
        {
            var nuevoLog = new HistorialLog
            {
                DescripcionLog = $"El usuario con ID '{idUsuarioLogueado}' a dado de alta el archivo con el id '{id_archivo}'.",
                TipoLog = "ALTA DE ARCHIVO",
                IdUsuario = idUsuarioLogueado,
                IpAddress = ObtenerIpLocal(),
                FechaLog = DateTime.UtcNow
            };

            _context.HistorialLogs.Add(nuevoLog);
            _context.SaveChanges();
        }

        //REGISTRAR MOVIMIENTO DE ECONOMICO
        public void RegistrarMovimientoEquipo(int idUsuarioLogueado, int idMovimiento)
        {
            var nuevoLog = new HistorialLog
            {
                DescripcionLog = $"El usuario con ID '{idUsuarioLogueado}' realizo el movimiento con el id '{idMovimiento}'.",
                TipoLog = "MOVIMIENTO DE ECONOMICO",
                IdUsuario = idUsuarioLogueado,
                IpAddress = ObtenerIpLocal(),
                FechaLog = DateTime.UtcNow
            };

            _context.HistorialLogs.Add(nuevoLog);
            _context.SaveChanges();
        }

        //REGISTRO SERVICIO DE EQUIPO

        public void RegistrarServicioEquipo(int idUsuarioLogueado, int idServicio)
        {
            var nuevoLog = new HistorialLog
            {
                DescripcionLog = $"El usuario con ID '{idUsuarioLogueado}' registro un nuevo servicio el id '{idServicio}'.",
                TipoLog = "REGISTRO DE SERVICIO",
                IdUsuario = idUsuarioLogueado,
                IpAddress = ObtenerIpLocal(),
                FechaLog = DateTime.UtcNow
            };

            _context.HistorialLogs.Add(nuevoLog);
            _context.SaveChanges();
        }

        //REGISTRO RESGUARDO DE EQUIPO

        public void RegistrarResguardoEquipo(int idUsuarioLogueado, int idResguardo)
        {
            var nuevoLog = new HistorialLog
            {
                DescripcionLog = $"El usuario con ID '{idUsuarioLogueado}' registro un nuevo resguardo el id '{idResguardo}'.",
                TipoLog = "REGISTRO DE RESGUARDO",
                IdUsuario = idUsuarioLogueado,
                IpAddress = ObtenerIpLocal(),
                FechaLog = DateTime.UtcNow
            };

            _context.HistorialLogs.Add(nuevoLog);
            _context.SaveChanges();
        }



    }
}