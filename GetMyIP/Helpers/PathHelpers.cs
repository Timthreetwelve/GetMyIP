// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

internal static class PathHelpers
{

    /// <summary>
    /// If a path to a file includes the user profile name replace it with %USERPROFILE%.
    /// </summary>
    /// <remarks>
    /// Users may not want to have their user names visible in the log file, especially when sending that log with a bug
    /// report. This method accomplishes that while still keeping the logged path usable.
    /// </remarks>
    /// <returns>
    /// A string representing the path.
    /// </returns>
    public static string AnonymizePath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }

        string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (string.IsNullOrEmpty(userProfile))
        {
            return path;
        }

        if (!path.StartsWith(userProfile, StringComparison.OrdinalIgnoreCase))
        {
            return path;
        }

        // Avoid partial prefix matches (e.g., "C:\Users\Bob" vs "C:\Users\Bobby")
        if (path.Length > userProfile.Length &&
            path[userProfile.Length] != Path.DirectorySeparatorChar &&
            path[userProfile.Length] != Path.AltDirectorySeparatorChar)
        {
            return path;
        }

        return $"%USERPROFILE%{path[userProfile.Length..]}";
    }
}
