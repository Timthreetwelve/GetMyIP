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

        InitializeRefreshTime();
    }
    #endregion Initialize settings

    #region Initialize refresh time
    /// <summary>
    /// Initializes the refresh time for the TimeSpinner control based on the AutoRefreshSeconds setting.
    /// </summary>
    private static void InitializeRefreshTime()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(UserSettings.Setting!.AutoRefreshSeconds);
        UserSettings.Setting.RefreshHours = timeSpan.Hours;
        UserSettings.Setting.RefreshMinutes = timeSpan.Minutes;
        UserSettings.Setting.RefreshSeconds = timeSpan.Seconds;
    }
    #endregion Initialize refresh time

    #region Read setting from file
    /// <summary>
    /// Read settings from JSON file.
    /// </summary>
    /// <returns>UserSettings</returns>
    private static UserSettings ReadConfiguration()
    {
        try
        {
            string json = File.ReadAllText(SettingsFileName!);
            UserSettings settings = JsonSerializer.Deserialize<UserSettings>(json)!;
            MigrateLegacyRefreshInterval(settings,json);
            return settings;
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

    #region Migrate legacy refresh interval
    /// <summary>
    /// Converts a settings file created before the AutoRefreshInterval enum was replaced
    /// with separate Hours/Minutes/Seconds properties. The old value was persisted as the
    /// total number of minutes (the underlying int value of the RefreshIntervals enum).
    /// </summary>
    /// <param name="json">The raw JSON text read from the settings file.</param>
    /// <param name="settings">The already-deserialized settings instance to update in place.</param>
    private static void MigrateLegacyRefreshInterval(UserSettings settings, string json)
    {
        try
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("AutoRefreshInterval", out JsonElement legacyValue))
            {
                if (legacyValue.TryGetInt32(out int totalMinutes))
                {
                    settings.AutoRefreshSeconds = totalMinutes * 60;
                    string msg = $"Migrated legacy AutoRefreshInterval ({totalMinutes} minutes) to AutoRefreshSeconds ({settings.AutoRefreshSeconds} seconds).";
                    _log.Info(msg);
                }
                else
                {
                    string msg = "Could not migrate legacy AutoRefreshInterval setting. Using defaults.";
                    _log.Warn(msg);
                    WriteBootstrapFallback(msg);
                }
            }
        }
        catch (Exception ex)
        {
            string msg = $"Could not migrate legacy AutoRefreshInterval setting. Using defaults. {ex.Message}";
            _log.Warn(ex, msg);
            WriteBootstrapFallback(msg + "\n" + ex.Message);
        }
    }
    #endregion Migrate legacy refresh interval

    #region Write to fallback log
    /// <summary>
    /// Writes a simple fallback log entry to a startup fallback file in the temp folder.
    /// Used to guarantee persistence of critical startup diagnostics when full logging
    /// may not yet be configured.
    /// </summary>
    /// <param name="message">Message to write.</param>
    private static void WriteBootstrapFallback(string message)
    {
        try
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "T_K");
            Directory.CreateDirectory(tempDir);
            string fallbackFile = Path.Combine(tempDir, "GetMyIP.startup.fallback.log");
            string line = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} {message}{Environment.NewLine}";
            File.AppendAllText(fallbackFile, line);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Failed to write bootstrap fallback log.");
        }
    }
    #endregion Write to fallback log

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
                _log.Debug($"Exporting settings file to {PathHelpers.AnonymizePath(saveFile.FileName)}.");
                string json = JsonSerializer.Serialize(UserSettings.Setting, _options);
                File.WriteAllText(saveFile.FileName, json);
            }
        }
        catch (Exception ex)
        {
            _log.Debug($"Error exporting settings file. {ex.Message}");
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
                _log.Debug($"Importing settings file from {PathHelpers.AnonymizePath(importFile.FileName)}.");
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
            _log.Debug($"Error importing settings file. {ex.Message}");
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
}
