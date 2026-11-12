using Inventario.Core.Services;

namespace InventarioMaquinaria.Services
{
  
    public class SessionService : ISessionService
    {
        public string Username { get; set; } = string.Empty;
        public int IdRol { get; set; }
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Area { get; set; }
        public string FirmaPath { get; set; }
        public string Correoe { get; set; }


        public void CerrarSesion()
        {
            Username = string.Empty; 
            IdRol = 0;
            NombreCompleto = string.Empty;
            Area = string.Empty;
            Correoe = string.Empty;
        }
    }
}