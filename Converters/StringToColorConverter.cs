namespace BoleBiljart.Converters
{
    public class StringToColorConverter : IValueConverter
    {
        public Color TrueColor { get; set; } = Colors.White;
        public Color FalseColor { get; set; } = Colors.Yellow;

        public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string s)
            {
                bool invert = false;
                invert = parameter?.ToString() == "Invert";
                bool b = (s == "Player 1");
                if (invert)
                {
                    b = !b;
                }
                return b ? TrueColor : FalseColor;
            }

            return FalseColor;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Color c)
            {
                return c == TrueColor ? "Player 1" : "Player 2";
            }
            return "Player 2";
        }
    }
}
