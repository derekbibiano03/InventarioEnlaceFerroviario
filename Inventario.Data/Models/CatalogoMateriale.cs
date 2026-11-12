using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoMateriale
{
    public int IdMaterial { get; set; }

    public string? DescripcionCorta { get; set; }

    public string? DescripcionLarga { get; set; }

    public string? UnidadMedida { get; set; }

    public string? NoParte { get; set; }

    public int? Marca { get; set; }

    public string? IdClasificacion { get; set; }

    public string? IdSubclasificacion { get; set; }

    public string? Detalles { get; set; }

    public string? IdGrupo { get; set; }

    public virtual CatalogoClasificacione? IdClasificacionNavigation { get; set; }

    public virtual CatalogoGrupo? IdGrupoNavigation { get; set; }

    public virtual CatalogoSubclasificacione? IdSubclasificacionNavigation { get; set; }

    public virtual CatalogoMarca? MarcaNavigation { get; set; }
}
