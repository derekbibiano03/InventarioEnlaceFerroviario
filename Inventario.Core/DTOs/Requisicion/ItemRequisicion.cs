using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.DTOs.Requisicion
{
    public class ItemRequisicion
    {
        // Número de partida
        public int Partida { get; set; }

        // Descripción del repuesto o insumo
        public string Descripcion { get; set; } = string.Empty;

        // Número de parte o código de catálogo
        public string NoParte { get; set; } = string.Empty;

        // Unidad de medida (ej. Pza, Litro, Metro)
        public string Unidad { get; set; } = string.Empty;

        // Cantidad a solicitar
        public double Cantidad { get; set; }
    }
}
