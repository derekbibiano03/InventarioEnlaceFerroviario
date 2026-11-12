using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Text;

namespace Inventario.Data.Models
{
    public partial class HistorialLogs
    {
        [Key]
        [Column("id_log")]
        public int IdLog { get; set; }

        [Column("descripcion_log")]
        public string? DescripcionLog { get; set; }

        [Column("tipo_log")]
        public string? TipoLog { get; set; }

        [Column("id_usuario")]
        public int? IdUsuario { get; set; }

        [Column("fecha_log")]
        public DateTime? FechaLog { get; set; }

        [Column("ip_address")]
        public IPAddress? IpAddress { get; set; }

        // El atributo [ForeignKey] va sobre la propiedad de navegación, apuntando al campo entero 'IdUsuario'
        [ForeignKey("IdUsuario")]
        public virtual Usuario? IdUsuarioNavigation { get; set; }
    }
}