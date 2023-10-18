// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Converters;

/// <summary>
/// Converter that changes Spacing to Thickness
/// </summary>
/// <seealso cref="System.Windows.Data.IValueConverter" />
internal class SpacingConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }
        switch (UserSettings.Setting.RowSpacing)
        {
            case Spacing.Compact:
                return new Thickness(15, 2, 15, 1);
            case Spacing.Comfortable:
                return new Thickness(15, 6, 15, 6);
            default:
                return new Thickness(15, 9, 15, 9);
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}
