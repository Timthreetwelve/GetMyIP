// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Converters;

/// <summary>
/// Converts a count value to Visibility. Returns Visible when count is 0, Collapsed otherwise.
/// Use ConverterParameter="Inverted" to reverse the logic.
/// </summary>
public class CountToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        int count = value is int v ? v : 0;
        bool inverted = parameter?.ToString() == "Inverted";

        bool isEmpty = count == 0;
        bool showElement = inverted ? !isEmpty : isEmpty;

        return showElement ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}
