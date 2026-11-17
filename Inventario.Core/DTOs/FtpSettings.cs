using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.DTOs
{
    public class FtpSettings
    {
        public string Host { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
        public string DirectorioRemoto { get; set; }
        public string DirectorioRemotoResguardos { get; set; }
        public string DirectorioRemotoMovimientos { get; set; }
        public string DirectorioRemotoRequisiciones { get; set; }
    }
}
