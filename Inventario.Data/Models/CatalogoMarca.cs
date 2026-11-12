using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoMarca
{
    public int IdMarca { get; set; }

    public string? NombreMarca { get; set; }

    public virtual ICollection<CatalogoEconomico> CatalogoEconomicoIdMarcaNavigations { get; set; } = new List<CatalogoEconomico>();

    public virtual ICollection<CatalogoEconomico> CatalogoEconomicoMarcaMotorNavigations { get; set; } = new List<CatalogoEconomico>();

    public virtual ICollection<CatalogoMateriale> CatalogoMateriales { get; set; } = new List<CatalogoMateriale>();
}
