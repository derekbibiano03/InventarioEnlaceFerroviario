using Inventario.Data.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Inventario.Core
{
    public class ExcelExportService
    {
        private readonly InventarioContext _context;
        public ExcelExportService()
        {
            _context = new InventarioContext();
        }
        public ExcelExportService(InventarioContext context)
        {
            _context = context ?? new InventarioContext();
        }

        public byte[] GenerarExcelHistorialServicios(List<HistorialServicio> listaHistorial)
        {
            if (listaHistorial == null) return new byte[0];

            ExcelPackage.License.SetNonCommercialOrganization("GalloMeda");

            // Blindaje contra nulos al crear el diccionario de grupos
            var gruposDict = _context?.CatalogoGrupos?
                .Where(g => g.IdGrupo != null)
                .ToDictionary(g => g.IdGrupo.Trim().ToUpper(), g => g.DescripcionGrupo)
                ?? new Dictionary<string, string>();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Historial de Servicios");

                string[] cabeceras = {
                    "ID SERVICIO", "ECONOMICO", "FECHA DE MANTENIMIENTO",
                    "TIPO DE MANTENIMIENTO", "ANOTACIONES", "HR/KM REAL", "HR/KM ACTUAL",
                    "COSTO DEL SERVICIO", "UBICACION DE REALIZACION", "USUARIO", "DESCR. GRUPO", "ID GRUPO"
                };

                for (int i = 0; i < cabeceras.Length; i++)
                {
                    var cell = worksheet.Cells[1, i + 1];
                    cell.Value = cabeceras[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(220, 220, 220));
                }

                int fila = 2;

                foreach (var item in listaHistorial)
                {
                    if (item == null) continue;

                    worksheet.Cells[fila, 1].Value = item.IdServicio;
                    worksheet.Cells[fila, 2].Value = item.NoEconomico;

                    if (item.FechaMantenimiento != default)
                    {
                        worksheet.Cells[fila, 3].Value = item.FechaMantenimiento.ToDateTime(TimeOnly.MinValue);
                        worksheet.Cells[fila, 3].Style.Numberformat.Format = "dd/MM/yyyy";
                    }

                    worksheet.Cells[fila, 4].Value = item.TipoMantenimiento;
                    worksheet.Cells[fila, 5].Value = item.Anotaciones;
                    worksheet.Cells[fila, 6].Value = item.Horaskilometrosreales;
                    worksheet.Cells[fila, 7].Value = item.NoEconomicoNavigation?.Horometro;

                    if (item.Costos.HasValue)
                    {
                        worksheet.Cells[fila, 8].Value = item.Costos.Value;
                        worksheet.Cells[fila, 8].Style.Numberformat.Format = "$#,##0.00";
                    }

                    worksheet.Cells[fila, 9].Value = item.UbicacionRealizacionNavigation?.NombreProyecto;
                    worksheet.Cells[fila, 10].Value = item.IdUsuarioNavigation?.NombreCompleto;

                    string descGrupo = "";
                    if (!string.IsNullOrEmpty(item.IdGrupo))
                    {
                        var idUpper = item.IdGrupo.Trim().ToUpper();
                        if (gruposDict.ContainsKey(idUpper))
                        {
                            descGrupo = gruposDict[idUpper];
                        }
                    }
                    worksheet.Cells[fila, 11].Value = descGrupo;
                    worksheet.Cells[fila, 12].Value = item.IdGrupo;

                    fila++;
                }

                worksheet.Cells[worksheet.Dimension?.Address ?? "A1"].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        public byte[] GenerarExcelEconomicos(List<CatalogoEconomico> listaFiltrada)
        {
            if (listaFiltrada == null) return new byte[0];

            ExcelPackage.License.SetNonCommercialOrganization("GalloMeda");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Inventario Completo");

                string[] cabeceras = {
                    "Económico ID", "Descripción", "Marca",
                    "Modelo", "Serie", "Periodo de Fabricacion", "Informacion de Motor",
                    "Marca de motor", "Modelo del Motor", "No. Serie del motor",
                    "Familia del motor", "Horometro", "Grado de Propiedad",
                    "Observaciones", "Estatus Seguro", "Tipo de Equipo", "Grupo", "Ubicación",
                    "Combustible", "Propietario", "Administrador", "Estatus",
                    "Operador", "Responsable", "Placas"
                };

                for (int i = 0; i < cabeceras.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = cabeceras[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                int fila = 2;

                foreach (var item in listaFiltrada)
                {
                    if (item == null) continue;

                    worksheet.Cells[fila, 1].Value = item.IdEconomico?.ToUpper() ?? "N/A";
                    worksheet.Cells[fila, 2].Value = item.Descripcion;
                    worksheet.Cells[fila, 3].Value = item.IdMarcaNavigation?.NombreMarca ?? "SIN INFORMACION";
                    worksheet.Cells[fila, 4].Value = item.Modelo;
                    worksheet.Cells[fila, 5].Value = item.Serie;
                    worksheet.Cells[fila, 6].Value = item.PeriodoFabricacion;
                    worksheet.Cells[fila, 7].Value = item.Motor;
                    worksheet.Cells[fila, 8].Value = item.MarcaMotorNavigation?.NombreMarca ?? "SIN IDENTIFICAR";
                    worksheet.Cells[fila, 9].Value = item.ModeloMotor;
                    worksheet.Cells[fila, 10].Value = item.SerieMotor;
                    worksheet.Cells[fila, 11].Value = item.FamiliaMotor;
                    worksheet.Cells[fila, 12].Value = item.Horometro;
                    worksheet.Cells[fila, 13].Value = item.GradoPropiedad;
                    worksheet.Cells[fila, 14].Value = item.ObservacionesAsignaciones;
                    worksheet.Cells[fila, 15].Value = item.EstatusSeguro;
                    worksheet.Cells[fila, 16].Value = item.IdTipoEquipoNavigation?.DescripcionTipoEquipo ?? "N/A";
                    worksheet.Cells[fila, 17].Value = item.IdGrupoNavigation?.DescripcionGrupo ?? "N/A";
                    worksheet.Cells[fila, 18].Value = item.IdUbicacionNavigation?.NombreProyecto ?? "N/A";
                    worksheet.Cells[fila, 19].Value = item.IdCombustibleNavigation?.DescripcionCombustible ?? "N/A";
                    worksheet.Cells[fila, 20].Value = item.IdPropietarioNavigation?.Nombre ?? "N/A";
                    worksheet.Cells[fila, 21].Value = item.IdAdministradorNavigation?.Nombre ?? "N/A";
                    worksheet.Cells[fila, 22].Value = item.IdEstatusNavigation?.DescripcionEstatus ?? "N/A";
                    worksheet.Cells[fila, 23].Value = item.IdOperadorNavigation?.NombreEmpleado ?? "N/A";
                    worksheet.Cells[fila, 24].Value = item.IdResponsableNavigation?.NombreEmpleado ?? "N/A";
                    worksheet.Cells[fila, 25].Value = item.Placas;

                    fila++;
                }

                worksheet.Cells[worksheet.Dimension?.Address ?? "A1"].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        public byte[] GenerarExcelRequisiciones(List<Requisicione> listaFiltrada)
        {
            if (listaFiltrada == null) return new byte[0];

            ExcelPackage.License.SetNonCommercialOrganization("GalloMeda");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Reporte de Requisiciones");

                string[] cabeceras = {
            "No. Requisicion", "Fecha", "Empresa",
            "Estatus", "Solicitante", "Ubicacion", "Economico"
        };

                for (int i = 0; i < cabeceras.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = cabeceras[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                int fila = 2;

                foreach (var item in listaFiltrada)
                {
                    if (item == null) continue;

                    worksheet.Cells[fila, 1].Value = item.IdRequisicion?.ToUpper() ?? "N/A";
                    worksheet.Cells[fila, 2].Value = item.FechaRequisicion;
                    worksheet.Cells[fila, 3].Value = item.Empresa ?? "N/A";
                    worksheet.Cells[fila, 4].Value = item.Estatus ?? "N/A";

                    // Protección contra objetos nulos
                    worksheet.Cells[fila, 5].Value = item.IdSolicitanteNavigation?.NombreCompleto ?? "N/A";
                    worksheet.Cells[fila, 6].Value = item.IdUbicacionNavigation?.Ubicacion ?? "N/A";

                    worksheet.Cells[fila, 7].Value = item.IdEconomico ?? "N/A";

                    fila++;
                }

                worksheet.Cells[worksheet.Dimension?.Address ?? "A1"].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }


    }
}