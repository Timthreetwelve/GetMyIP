// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

/// <summary>
/// Class for localization and culture helper methods.
/// </summary>
internal static class LocalizationHelpers
{
    #region Get the current Culture
    /// <summary>
    /// Gets the current culture.
    /// </summary>
    /// <returns>Current culture name</returns>
    public static string GetCurrentCulture()
    {
        return CultureInfo.CurrentCulture.Name;
    }
    #endregion Get the current Culture

    #region Get the current UI Culture
    /// <summary>
    /// Gets the current UI culture.
    /// </summary>
    /// <returns>Current UI culture name</returns>
    public static string GetCurrentUICulture()
    {
        return CultureInfo.CurrentUICulture.Name;
    }
    #endregion Get the current UI Culture

    #region Save and Restart
    /// <summary>
    /// Saves settings and restarts the application. Invoked when language is changed.
    /// </summary>
    public static void SaveAndRestart()
    {
        ConfigHelpers.SaveSettings();
        using Process p = new();
        p.StartInfo.FileName = AppInfo.AppPath;
        p.StartInfo.UseShellExecute = true;
        _ = p.Start();
        _log.Debug("Restarting for language change.");
        Application.Current.Shutdown();
    }
    #endregion Save and Restart
}
