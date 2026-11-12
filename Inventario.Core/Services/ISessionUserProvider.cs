using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.Services
{
    public interface ISessionUserProvider
    {
        // Devuelve el identificador único del usuario que se encuentra logueado en el sistema
        int GetCurrentUserId();
    }
}
