// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Configuration;

/// <summary>
/// Class for methods used for creating, reading and saving settings.
/// </summary>
public static class ConfigHelpers
{
    #region Properties & fields
    public static string? SettingsFileName { get; private set; }
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };
    #endregion Properties & fields

    #region MainWindow Instance
    private static readonly MainWindow? _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region Initialize settings
    /// <summary>
    ///  Initialization method. Gets the file name for settings file and creates it if it
    ///  doesn't exist.
    /// </summary>
    /// <param name="settingsFile">Option name of settings file</param>
    public static void InitializeSettings(string settingsFile = "usersettings.json")
    {
        string? settingsDir = Path.GetDirectoryName(AppContext.BaseDirectory);
        SettingsFileName = Path.Combine(settingsDir!, settingsFile);

        if (!File.Exists(SettingsFileName))
        {
            UserSettings.Setting = new UserSettings();
            SaveSettings();
        }
        ConfigManager<UserSettings>.Setting = ReadConfiguration();

        ConfigManager<TempSettings>.Setting = new TempSettings();
    }
    #endregion Initialize settings

    #region Read setting from file
    /// <summary>
    /// Read settings from JSON file.
    /// </summary>
    /// <returns>UserSettings</returns>
    private static UserSettings ReadConfiguration()
    {
        try
        {
            return JsonSerializer.Deserialize<UserSettings>(File.ReadAllText(SettingsFileName!))!;
        }
        catch (Exception ex)
        {
            _ = MessageBox.Show($"{GetStringResource("MsgText_Error_Settings")}\n\n {ex.Message}",
                     GetStringResource("MsgText_Error_Caption"),
                     MessageBoxButton.OK,
                     MessageBoxImage.Error);
            return new UserSettings();
        }
    }
    #endregion Read setting from file

    #region Save settings to JSON file
    /// <summary>
    /// Write settings to JSON file.
    /// </summary>
    public static void SaveSettings()
    {
        try
        {
            string json = JsonSerializer.Serialize(UserSettings.Setting, _options);
            File.WriteAllText(SettingsFileName!, json);
        }
        catch (Exception ex)
        {
            _ = MessageBox.Show($"{GetStringResource("MsgText_Error_SavingSettings")}\n{ex.Message}",
                     GetStringResource("MsgText_Error_Caption"),
                     MessageBoxButton.OK,
                     MessageBoxImage.Error);
        }
    }
    #endregion Save settings to JSON file

    #region Export settings
    /// <summary>
    /// Exports the current settings to a JSON file.
    /// </summary>
    public static void ExportSettings()
    {
        try
        {
            string appPart = AppInfo.AppProduct.Replace(" ", "");
            string settingsPart = GetStringResource("NavItem_Settings");
            string datePart = DateTime.Now.ToString("yyyyMMdd", CultureInfo.CurrentCulture);
            SaveFileDialog saveFile = new()
            {
                CheckPathExists = true,
                Filter = "JSON File|*.json|All Files|*.*",
                FileName = $"{appPart}_{settingsPart}_{datePart}.json"
            };

            if (saveFile.ShowDialog() == true)
            {
                _log.Debug($"Exporting settings file to {saveFile.FileName}.");
                string json = JsonSerializer.Serialize(UserSettings.Setting, _options);
                File.WriteAllText(saveFile.FileName, json);
            }
        }
        catch (Exception ex)
        {
            _log.Debug(ex, "Error exporting settings file.");
            _ = MessageBox.Show($"{GetStringResource("MsgText_Error_ExportingSettings")}\n{ex.Message}",
                    GetStringResource("MsgText_ErrorCaption"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }
    }
    #endregion Export settings

    #region Import settings
    /// <summary>
    /// Imports settings from a previously exported file.
    /// </summary>
    public static void ImportSettings()
    {
        try
        {
            OpenFileDialog importFile = new()
            {
                CheckPathExists = true,
                CheckFileExists = true,
                Filter = "JSON File|*.json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (importFile.ShowDialog() == true)
            {
                _log.Debug($"Importing settings file from {importFile.FileName}.");
                ConfigManager<UserSettings>.Setting = JsonSerializer.Deserialize<UserSettings>(File.ReadAllText(importFile.FileName))!;
                SaveSettings();

                _ = new MDCustMsgBox($"{GetStringResource("MsgText_ImportSettingsRestart")}",
                "Get My IP",
                ButtonType.Ok,
                false,
                true,
                _mainWindow).ShowDialog();
            }
        }
        catch (Exception ex)
        {
            _log.Debug(ex, "Error importing settings file.");
            _ = MessageBox.Show($"{GetStringResource("MsgText_Error_ImportingSettings")}\n{ex.Message}",
                    GetStringResource("MsgText_ErrorCaption"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }
    }
    #endregion Import settings

    #region Dump settings into the log
    /// <summary>
    /// Dumps (writes) current settings to the log file.
    /// </summary>
    public static void DumpSettings()
    {
        string dashes = new('-', 25);
        string header = $"{dashes} Begin Settings {dashes}";
        string trailer = $"{dashes} End Settings {dashes}";
        _log.Debug(header);
        PropertyInfo[] properties = typeof(UserSettings).GetProperties();
        int maxLength = properties.Max(s => s.Name.Length);
        foreach (PropertyInfo property in properties)
        {
            string? value = property.GetValue(UserSettings.Setting, [])!.ToString();
            _log.Debug($"{property.Name.PadRight(maxLength)} : {value}");
        }
        _log.Debug(trailer);
    }
    #endregion Dump settings into the log

    #region Restart the application
    /// <summary>
    /// Restarts the app to completely utilize imported settings.
    /// </summary>
    private static void RestartApp()
    {
        string script = Path.Combine(AppInfo.AppDirectory, "PowerShell", "Restart.ps1");
        Process p = new();
        p.StartInfo.FileName = "powershell.exe";
        p.StartInfo.Arguments = $"-NoProfile -File \"{script}\"";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;

        _ = p.Start();
        _log.Debug("Restarting after settings import.");
        App.ExplicitClose = true;
        Application.Current.Shutdown();
    }
    #endregion Restart the application
}
