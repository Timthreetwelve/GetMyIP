// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using GetMyIP.Models;

namespace GetMyIP;

public static class EnumHelper
{
    /// <summary>
    /// Gets the MDIX accent color names.
    /// </summary>
    /// <returns>An array of color names</returns>
    /// <remarks>Use this to get a name like "Light Blue" rather than "LightBlue"</remarks>
    public static string[] GetColors()
    {
        List<string> colors = new();
        foreach (Enum i in Enum.GetValues(typeof(AccentColor)))
        {
            colors.Add(i.GetDescription());
        }
        return colors.ToArray();
    }

    /// <summary>
    /// Gets the description of the enum if available. Otherwise gets the field name.
    /// </summary>
    /// <param name="enumValue">The enum value.</param>
    /// <returns>The enum description</returns>
    public static string GetDescription(this Enum enumValue)
    {
        FieldInfo field = enumValue.GetType().GetField(enumValue.ToString());
        if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
        {
            return attribute.Description;
        }
        return field.Name;
    }
}