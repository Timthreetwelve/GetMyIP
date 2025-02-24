﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

/// <summary>
/// Class to allow boolean values to be converted to localized "true"/"false" or "yes"/"no" strings.
/// </summary>
internal static class BooleanHelpers
{
    /// <summary>
    /// Convert boolean True or False to localized "Yes" or "No" string.
    /// </summary>
    /// <returns>Localized string.</returns>
    public static string ToYesNoString(this bool value)
    {
        return value ? GetStringResource("MsgText_BooleanYes") : GetStringResource("MsgText_BooleanNo");
    }
}
