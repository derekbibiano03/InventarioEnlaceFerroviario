using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class HistorialLog
{
    public int IdLog { get; set; }

    public string? DescripcionLog { get; set; }

    public string? TipoLog { get; set; }

    public int? IdUsuario { get; set; }

    public DateTime? FechaLog { get; set; }

    public string IpAddress { get; set; } = null!;

    public string? UserAgent { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
