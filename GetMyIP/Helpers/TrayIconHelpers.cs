// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

// System.Drawing should not be added to global usings
using System.Drawing;

namespace GetMyIP.Helpers;
internal static class TrayIconHelpers
{
    #region MainWindow Instance
    private static readonly MainWindow? _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region Private properties
    internal static string CurrentCountryCode { get; set; } = string.Empty;
    #endregion Private properties

    #region Set the tray icon
    /// <summary>
    /// Set the tray icon to the default "IP" icon or flag Icon.
    /// </summary>
    internal static void SetTrayIcon()
    {
        string? countryCode = GetCountryCode();
        if (UserSettings.Setting!.ShowFlagIcon)
        {
            if (countryCode == CurrentCountryCode)
            {
                return;
            }
            if (countryCode != null)
            {
                _ = Application.Current.Dispatcher.Invoke(() => _mainWindow!.TbIcon.Icon = GetCountryFlag(countryCode));
                CurrentCountryCode = countryCode;
            }
            else
            {
                _ = Application.Current.Dispatcher.Invoke(() => _mainWindow!.TbIcon.Icon = GetDefaultIcon());
                CurrentCountryCode = "default";
            }
        }
        else
        {
            _ = Application.Current.Dispatcher.Invoke(() => _mainWindow!.TbIcon.Icon = GetDefaultIcon());
            CurrentCountryCode = "default";
        }
    }
    #endregion Set the tray icon

    #region Get the flag icon
    /// <summary>
    /// Gets the flag icon for the corresponding country code.
    /// </summary>
    /// <param name="country">Two-character country code.</param>
    /// <returns>Icon to be used by TaskBarIcon.Icon</returns>
    internal static Icon GetCountryFlag(string country)
    {
        string iconFile = $"pack://application:,,,/Images/Flags/{country}.ico";
        try
        {
            _log.Debug($"Loading \"{country}\" flag icon");
            return new Icon(Application.GetResourceStream(new Uri(iconFile)).Stream);
        }
        catch (Exception ex)
        {
            _log.Error(ex, $"Unable to load flag icon for \"{country}\"");
            return GetDefaultIcon();
        }
    }
    #endregion Get the flag icon

    #region Get the default "IP" icon
    /// <summary>
    /// Gets the default icon.
    /// </summary>
    /// <returns>The "IP" icon</returns>
    internal static Icon GetDefaultIcon()
    {
        return new Icon(Application.GetResourceStream(new Uri("pack://application:,,,/images/ip.ico")).Stream);
    }
    #endregion Get the default "IP" icon

    #region Get the current country code
    /// <summary>
    /// Gets the two-character country code.
    /// </summary>
    /// <returns>A two-character country code</returns>
    internal static string? GetCountryCode()
    {
        return IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == GetStringResource("External_CountryCode"))?.Value;
    }
    #endregion Get the current country code
}
