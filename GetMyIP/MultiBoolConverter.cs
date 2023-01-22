// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System.Globalization;

namespace GetMyIP;

internal class MultiBoolConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        foreach (object value in values)
        {
            if (value is bool x && !x)
            {
                return false;
            }
        }
        return true;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
