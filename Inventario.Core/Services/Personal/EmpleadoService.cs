using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.Services.Personal
{
    public class EmpleadoService
    {
        private readonly InventarioContext _context;
        public EmpleadoService(InventarioContext context)
        {
            _context = context;
        }

        public List<Empleado> ObtenerResponsables(List<int> idsRolFiltro = null)
        {
            var resultado = _context.Empleados
                .Where(e => idsRolFiltro == null || !idsRolFiltro.Any() || (e.IdRolEmpleado.HasValue && idsRolFiltro.Contains(e.IdRolEmpleado.Value)))
                .Select(e => new Empleado
                {
                    NoEmpleado = e.NoEmpleado,
                    NombreEmpleado = e.NombreEmpleado
                })
                .OrderBy(x => x.NombreEmpleado).ToList();

            return resultado;
        }
        public List<Empleado> ObtenerOperadores()
        {

            var resultado = _context.Empleados
                .Select(e => new Empleado
                {
                    NoEmpleado = e.NoEmpleado,
                    NombreEmpleado = e.NombreEmpleado
                })
                .OrderBy(x => x.NombreEmpleado)
                .ToList();

            return resultado;
        }


        public List<Empleado> ObtenerOperadoresConFechaNoNula()
        {
            var resultado = _context.Empleados
                .Select(e => new Empleado
                {
                    NoEmpleado = e.NoEmpleado,
                    NombreEmpleado = e.NombreEmpleado,
                    Ds3 = e.Ds3
                })
                .OrderBy (x => x.NombreEmpleado)
                .ToList();

            return resultado;
        }
    }
}
