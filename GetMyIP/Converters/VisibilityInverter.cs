// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Converters;

/// <summary>
/// An inverse visibility converter
/// </summary>
internal sealed class VisibilityInverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return Visibility.Visible;
        }
        if (value is Visibility visibilityValue)
        {
            return visibilityValue == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
        throw new InvalidOperationException("The value must be a Visibility.");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}
