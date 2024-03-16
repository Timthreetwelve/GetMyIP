// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Converters;

/// <summary>
/// Allows a Description Attribute in an Enum to be localized
/// </summary>
/// <param name="resourceKey">The resource key</param>
/// <remarks>
/// Based on https://brianlagunas.com/localize-enum-descriptions-in-wpf/
/// </remarks>
internal class LocalizedDescriptionAttribute(string resourceKey) : DescriptionAttribute
{
    private readonly string _resourceKey = resourceKey;

    public override string Description
    {
        get
        {
            object description;
            try
            {
                description = Application.Current.TryFindResource(_resourceKey);
            }
            catch (Exception)
            {
                return $"{_resourceKey} value is null";
            }

            if (description is null)
            {
                return $"{_resourceKey} resource not found";
            }

            return description.ToString();
        }
    }
}
