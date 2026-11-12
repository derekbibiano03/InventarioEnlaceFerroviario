using Inventario.Data.Models; // Importa los modelos donde habita CatalogoEconomico
using System; // Importa los tipos base del sistema
using System.Globalization; // Importa clases para formateo de texto y cultura
using System.Windows.Data; // Importa la interfaz IValueConverter de WPF

namespace Inventario.Desktop.Helpers // Define el namespace coincidente con el XAML
{ // Inicio del namespace
    public class UnidadesHorometroConverter : IValueConverter // CORREGIDO: Debe ser public e implementar IValueConverter
    { // Inicio de la clase
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) // Transforma los datos recibidos desde el Binding
        { // Inicio del método Convert
            if (value == null) // Verifica si el objeto recibido desde el DataBinding es nulo
            { // Inicio del bloque
                return string.Empty; // Retorna una cadena vacía para prevenir excepciones en pantalla
            } // Fin del bloque

            if (value is CatalogoEconomico detalle) // Evalúa si el objeto es del tipo CatalogoEconomico y crea la variable 'detalle'
            { // Inicio del bloque
                decimal valorNumerico = detalle.Horometro; // Extrae el Horometro directamente (es decimal en el modelo)

                string unidad = (detalle.Thk != null && detalle.Thk.Equals("KILOMETRAJE", StringComparison.OrdinalIgnoreCase)) ? "Km" : "hrs"; // Evalúa si es kilometraje o horas

                return $"{valorNumerico:N0} {unidad}"; // Formatea el número con separador de miles y sufijo
            } // Fin del bloque

            return value.ToString() ?? string.Empty; // Si no es CatalogoEconomico, devuelve la representación en texto por defecto
        } // Fin del método Convert

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) // Método de conversión inversa
        { // Inicio del método
            throw new NotImplementedException(); // Lanza excepción indicando que solo es de lectura
        } // Fin del método
    } // Fin de la clase
} // Fin del namespace