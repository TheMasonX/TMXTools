using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace TMXTools.WPF.Converters;

[ValueConversion(typeof(InputGesture), typeof(string))]
class GestureConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not InputGesture gesture)
        {
            return value?.ToString() ?? DependencyProperty.UnsetValue;
        }

        if (value is KeyGesture keyGesture)
        {
            return keyGesture.GetDisplayStringForCulture(CultureInfo.CurrentCulture);
        }
        else if (value is MouseGesture mouseGesture)
        {
            return $"{mouseGesture.Modifiers} + {mouseGesture.MouseAction}";
        }

        return value.ToString() ?? DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        string? strValue = value as string;
        if (string.IsNullOrEmpty(strValue))
        {
            return DependencyProperty.UnsetValue;
        }

        if (targetType.IsAssignableTo(typeof(KeyGesture)))
        {
            var converter = new KeyGestureConverter();
            if (converter.ConvertFrom(strValue!) is KeyGesture keyGesture)
            {
                return keyGesture;
            }
        }

        if (targetType.IsAssignableTo(typeof(MouseGesture)))
        {
            var converter = new MouseGestureConverter();
            if (converter.ConvertFrom(strValue!) is MouseGesture mouseGesture)
            {
                return mouseGesture;
            }
        }

        return DependencyProperty.UnsetValue;
    }
}