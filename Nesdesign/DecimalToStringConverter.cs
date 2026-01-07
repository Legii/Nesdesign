using System;
using System.Globalization;
using System.Windows.Data;

namespace Nesdesign
{
    public class DecimalToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            if (value is decimal d)
                return d.ToString(culture);

            if (value is IFormattable f)
                return f.ToString(null, culture);

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = (value as string ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(input))
                return null;

            // separator z bieżącej kultury (fallback na CurrentCulture)
            var c = culture ?? CultureInfo.CurrentCulture;
            var sep = c.NumberFormat.NumberDecimalSeparator;

            // jeśli użytkownik właśnie wpisuje końcowy separator (np. "12."), nie nadpisujemy pola
            if (input.EndsWith(".") || input.EndsWith(","))
                return Binding.DoNothing;

            // zamień wszystkie możliwe separatoty na separator kultury
            input = input.Replace(".", sep).Replace(",", sep);

            var style = NumberStyles.Number;

            if (decimal.TryParse(input, style, c, out var result))
                return (decimal?)result;

            // w razie błędu parsowania nie nadpisuj pola
            return Binding.DoNothing;
        }
    }
}
