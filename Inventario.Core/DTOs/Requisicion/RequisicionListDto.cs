using System;

namespace Inventario.Core.DTOs.Requisicion
{
    public class RequisicionListDto
    {
        public string IdRequisicion { get; set; }
        public DateOnly FechaRequisicion { get; set; }
        public string FechaTexto => FechaRequisicion.ToString("yyyy-MM-dd");

        public int IdUbicacion { get; set; }
        public string Empresa { get; set; }
        public string Ubicacion { get; set; }
        public string IdEconomico { get; set; }
        public string GradoPrioridad { get; set; }
        public string Estatus { get; set; }
        public string Solicitante { get; set; }
        public string Autorizante { get; set; }
        public string Atencion { get; set; }
        public string TipoRequisicion { get; set; }
        public string Reviso { get; set; }
        public string ArchivoReq { get; set; }
        public string CorreoAtencion { get; set; }
        public string CorreoSolicitante { get; set; }

    }
}