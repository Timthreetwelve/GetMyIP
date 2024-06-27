// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Converters;
/// <summary>
/// Enum description converter
/// </summary>
internal class EnumDescConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Enum myEnum = (Enum)value;
        if (myEnum == null)
        {
            return null;
        }
        string description = GetEnumDescription(myEnum);
        if (!string.IsNullOrEmpty(description))
        {
            return description;
        }
        return myEnum.ToString();
    }

    internal static string GetEnumDescription(Enum enumObj)
    {
        if (enumObj == null)
        {
            return string.Empty;
        }
        FieldInfo? field = enumObj.GetType().GetField(enumObj.ToString());
        object[] attrArray = field!.GetCustomAttributes(false);

        if (attrArray.Length > 0)
        {
            DescriptionAttribute? attribute = attrArray[0] as DescriptionAttribute;
            return attribute!.Description;
        }
        else
        {
            return enumObj.ToString();
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return string.Empty;
    }
}
