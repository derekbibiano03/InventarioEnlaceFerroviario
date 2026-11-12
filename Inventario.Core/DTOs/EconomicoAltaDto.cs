namespace Inventario.Core.DTOs
{
    public class EconomicoAltaDto
    {
        public required string IdTipoEquipo { get; set; }
        public required string IdGrupo { get; set; }
        public required int IdCombustible { get; set; }
        public required int IdPropietario { get; set; }
        public required int IdAdministrador { get; set; }
        public required int IdUbicacion { get; set; }
        public required int IdOperador { get; set; }
        public required int IdResponsable { get; set; }
        public required string GradoPropiedad { get; set; }
        public string? Observaciones { get; set; }
        public required int IdMarca { get; set; }
        public required string Modelo { get; set; }
        public required string Serie { get; set; }
        public int? PeriodoFab { get; set; }
        public required int MarcaMotor { get; set; }
        public string? ModeloMotor { get; set; }
        public string? SerieMotor { get; set; }
        public string? FamiliaMotor { get; set; }
        public string? Placas { get; set; }
        public string? PolizaAdj { get; set; }
        public required string THK { get; set; }
        public decimal Horometro { get; set; }
        public string? Dimensiones { get; set; }

        // 4. Booleano del seguro
        public required bool EstatusSeguro { get; set; }
        public string? TipoSeguro { get; set; }
    }
}