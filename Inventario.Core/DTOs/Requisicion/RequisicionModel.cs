using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Inventario.Core.DTOs.Requisicion
{
    public class RequisicionModel
    {
        // Datos de Encabezado
        public string AtencionA { get; set; } = string.Empty;
        public string DepartamentoAtencion { get; set; } = string.Empty;
        public string Solicitante { get; set; } = string.Empty;
        public string DepartamentoSolicitante { get; set; } = string.Empty;
        public string ObraSolicitante { get; set; } = string.Empty;
        public string AreaSolicitante { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;

        // Datos del Activo / Maquinaria
        public string NoEconomico { get; set; } = string.Empty;
        public string DescripcionEconomico { get; set; } = string.Empty;
        public string Motor { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string NoSerie { get; set; } = string.Empty;

        // Lista de compras
        public ObservableCollection<ItemRequisicion> Items { get; set; } = new ObservableCollection<ItemRequisicion>();

        // Observaciones y Autorización
        public string Observaciones { get; set; } = string.Empty;
        public string Autorizante { get; set; } = string.Empty;
    }
}
