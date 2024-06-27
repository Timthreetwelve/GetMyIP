// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Converters;

/// <summary>
/// Used to make sure the navigation ListBox selected item is correct
/// </summary>
/// <seealso cref="System.Windows.Data.IValueConverter" />
internal class SelectedItemConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is NavigationItem navigationItem)
        {
            return navigationItem;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}
