using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoPya
{
    public int IdPya { get; set; }

    public int? IdRolPya { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<CatalogoEconomico> CatalogoEconomicoIdAdministradorNavigations { get; set; } = new List<CatalogoEconomico>();

    public virtual ICollection<CatalogoEconomico> CatalogoEconomicoIdPropietarioNavigations { get; set; } = new List<CatalogoEconomico>();

    public virtual CatalogoRolPya? IdRolPyaNavigation { get; set; }
}
