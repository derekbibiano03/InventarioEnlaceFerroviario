using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class UsuriosRole
{
    public int IdLog { get; set; }

    public string? InformacionLog { get; set; }

    public string? TipoLog { get; set; }

    public int? IdUsuario { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
