// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Converters;

/// <summary>
/// Allows a Description Attribute in an Enum to be localized
/// </summary>
/// <param name="resourceKey">The resource key</param>
/// <remarks>
/// Based on https://brianlagunas.com/localize-enum-descriptions-in-wpf/
/// </remarks>
internal sealed class LocalizedDescriptionAttribute(string resourceKey) : DescriptionAttribute
{
    public override string Description
    {
        get
        {
            object description;
            try
            {
                description = Application.Current.TryFindResource(resourceKey);
            }
            catch (Exception)
            {
                return $"{resourceKey} value is null";
            }

            if (description is null)
            {
                return $"{resourceKey} resource not found";
            }

            return description.ToString()!;
        }
    }
}
