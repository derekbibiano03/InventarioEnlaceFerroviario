namespace Inventario.Core.DTOs.Requisicion
{
    public class ItemRequisicion
    {
        public int Partida { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string NoParte { get; set; } = string.Empty;
        public string Unidad { get; set; } = string.Empty;
        public double Cantidad { get; set; }
    }
}
