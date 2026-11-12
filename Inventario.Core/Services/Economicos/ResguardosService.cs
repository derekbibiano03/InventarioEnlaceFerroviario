using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Inventario.Core.Services.Economicos
{
    public class ResguardosService
    {
        private readonly InventarioContext _context;
        private readonly LogsService _logsService;
        private readonly string _hostServidor = "170.10.162.13";
        private readonly string _usuarioFtp = "dbibiano@enlaceferroviario.com";
        private readonly string _contrasenaFtp = "drbr11122003DRBR.";

        public ResguardosService(InventarioContext context, LogsService logsService)
        {
            _context = context;
            _logsService = logsService;
        }

        public HistorialResguardo registrarResguardo(int idUsuarioOperativo, HistorialResguardo dto)
        {
            var nuevoResguardo = new HistorialResguardo
            {
                IdEconomico = dto.IdEconomico,
                IdOperador = dto.IdOperador,
                FechaResguardo = dto.FechaResguardo,
                Observaciones = dto.Observaciones,
            };

            _context.HistorialResguardos.Add(nuevoResguardo);
            _context.SaveChanges();

            int idResguardo = nuevoResguardo.IdResguardo;
            _logsService.RegistrarServicioEquipo(idUsuarioOperativo, idResguardo);

            return nuevoResguardo;
        }

        public void GuardarArchivosRelacionados(List<(CatalogoArchivo archivoCatalogado, ResguardosArchivo relacionResguardo)> archivosProcesados)
        {
            foreach (var item in archivosProcesados)
            {
                _context.CatalogoArchivos.Add(item.archivoCatalogado);
                _context.ResguardosArchivos.Add(item.relacionResguardo);
            }

            _context.SaveChanges();
        }

        public List<HistorialResguardo> ObtenerHistorial()
        {
            return _context.HistorialResguardos
                .Include(s => s.IdEconomicoNavigation)
                .Include(h => h.IdOperadorNavigation)      // <-- Carga el usuario
                .Include(h => h.ResguardosArchivos)
                    .ThenInclude(sa => sa.IdArchivoNavigation)
                .ToList();
        }
    }
}