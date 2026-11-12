using System;
using System.Collections.Generic;
using System.Text;
using Inventario.Data.Models;

namespace Inventario.Core.Services.Adq_Serv.OrdenesCompra
{
    public class OrdenCompraService
    {
        private readonly InventarioContext _context;

        public OrdenCompraService(InventarioContext context)
        {
            _context = context;
        }



    }
}
