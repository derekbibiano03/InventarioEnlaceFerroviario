using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.Services
{
    public interface ISessionService
    {
        string Username { get; set; }
        int IdRol{ get; set; }
        int IdUsuario { get; set; } 
        string NombreCompleto { get; set; }
        string Area { get; set; }
        string FirmaPath { get; set; }
        string Correoe { get; set; }

        void CerrarSesion();
    }
}
