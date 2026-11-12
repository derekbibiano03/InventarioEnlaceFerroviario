using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Inventario.Desktop.Helpers
{
    public class RolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Si el valor es nulo, colapsa de inmediato
            if (value == null || parameter == null)
                return Visibility.Collapsed;

            // Convertimos el rol del usuario a string para evitar problemas de tipos (int, string, long)
            string idRolUsuarioStr = value.ToString()?.Trim();
            string rolesPermitidosStr = parameter.ToString()?.Trim();

            if (string.IsNullOrEmpty(idRolUsuarioStr) || string.IsNullOrEmpty(rolesPermitidosStr))
                return Visibility.Collapsed;

            // Separamos por guion '-' como acordamos
            var listaRoles = rolesPermitidosStr.Split('-');
            foreach (var rolStr in listaRoles)
            {
                if (rolStr.Trim() == idRolUsuarioStr)
                {
                    return Visibility.Visible;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}