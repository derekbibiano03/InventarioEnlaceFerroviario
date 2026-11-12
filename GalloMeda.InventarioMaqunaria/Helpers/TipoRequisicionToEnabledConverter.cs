using System;
using System.Globalization;
using System.Windows.Data;

namespace Inventario.Desktop.Helpers
{
    public class TipoRequisicionToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string tipo)
            {
                if (tipo == "REQUISICION PARA MATERIALES" || tipo == "REQUISICION PARA AGREGAR A STOCK" || tipo == "REQUISICION PARA HERRAMIENTAS" || tipo == "REQUISICION PARA OTROS")
                {
                    return false;
                }
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}