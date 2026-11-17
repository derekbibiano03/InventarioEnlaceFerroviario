using Inventario.Data.Models;
using System.Globalization; 
using System.Windows.Data; 

namespace Inventario.Desktop.Helpers 
{ 
    public class UnidadesHorometroConverter : IValueConverter 
    { 
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        { 
            if (value == null) 
            { 
                return string.Empty; 
            } 

            if (value is CatalogoEconomico detalle) 
            { 
                decimal valorNumerico = detalle.Horometro; 

                string unidad = (detalle.Thk != null && detalle.Thk.Equals("KILOMETRAJE", StringComparison.OrdinalIgnoreCase)) ? "Km" : "hrs"; 

                return $"{valorNumerico:N0} {unidad}"; 
            } 

            return value.ToString() ?? string.Empty; 
        } 

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        { 
            throw new NotImplementedException(); 
        } 
    }
} 