// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

internal static class JsonHelpers
{
    #region JSON options
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    #endregion JSON options

    #region Validate JSON
    /// <summary>
    /// Determines if string is valid JSON.
    /// </summary>
    /// <param name="jsonString">JSON string to check.</param>
    /// <returns>True if jsonString is valid.</returns>
    public static bool IsValid(string jsonString)
    {
        try
        {
            using (JsonDocument.Parse(jsonString))
            {
                return true;
            }
        }
        catch (JsonException)
        {
            return false;
        }
    }
    #endregion Validate JSON

    #region Truncate a JSON string
    /// <summary>
    /// Truncate a JSON string.
    /// </summary>
    /// <param name="jsonString">The JSON string to truncate.</param>
    /// <param name="maxChars">Maximum number of characters to return,</param>
    /// <returns>Empty string if jsonString was null or empty,
    /// the entire string if maxChars is 0,
    /// or the number of characters specified by maxChars or the entire JSON string, whichever is shorter.</returns>
    public static string TruncateJson(string jsonString, int maxChars)
    {
        if (string.IsNullOrEmpty(jsonString))
        {
            return string.Empty;
        }
        return maxChars == 0 ? jsonString : jsonString[..Math.Min(jsonString.Length, maxChars)];
    }
}
#endregion Truncate a JSON string
