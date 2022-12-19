// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP;

/// <summary>
/// A class and methods for reading, updating and saving user settings in a JSON file
/// </summary>
/// <typeparam name="T">Class name of user settings</typeparam>
public abstract class SettingsManager<T> where T : SettingsManager<T>, new()
{
    #region Properties
    private static SettingsFolder Folder { get; set; }
    private static string FileName { get; set; }
    private static string FilePath { get; set; }
    public static T Setting { get; private set; }
    #endregion Properties

    #region Settings Folder
    /// <summary>
    /// Settings folder location
    /// </summary>
    public enum SettingsFolder
    {
        /// <summary>
        /// The user's AppData\Roaming folder
        /// </summary>
        AppData = 1,
        /// <summary>
        /// The application's folder
        /// </summary>
        AppFolder = 2,
        /// <summary>
        /// The user's AppData\Local folder
        /// </summary>
        LocalAppData = 3
    }
    #endregion Settings Folder

    #region Initialization
    /// <summary>
    ///  Initialization method. Gets the file name for settings file and creates it if it
    ///  doesn't exist. Optionally loads the settings file.
    /// </summary>
    /// <param name="folder">Folder name can be a path or one of the const values</param>
    /// <param name="fileName">File name can be a file name (without path)</param>
    /// <param name="load">Read and load the settings file during initialization</param>
    public static void Init(SettingsFolder folder, string fileName = null, bool load = true)
    {
        Folder = folder;
        FileName = fileName;
        GetSettingsFile(Folder, FileName);
        if (!File.Exists(FilePath))
        {
            CreateNewSettingsJson(FilePath);
        }
        if (load)
        {
            LoadSettings();
        }
    }
    #endregion Initialization

    #region Get the settings file path
    /// <summary>
    /// Returns path to settings file. Accepts constants for folder and filename
    /// </summary>
    /// <param name="Folder"></param>
    /// <param name="FileName"></param>
    /// <returns>Path to settings file</returns>
    private static void GetSettingsFile(SettingsFolder Folder, string FileName)
    {
        string folderPath = null;
        string appName = Assembly.GetEntryAssembly().GetName().Name;
        string companyName = FileVersionInfo
                             .GetVersionInfo(Assembly.GetEntryAssembly().Location)
                             .CompanyName ?? string.Empty;

        switch (Folder)
        {
            case SettingsFolder.LocalAppData:
                folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    companyName, appName);
                break;
            case SettingsFolder.AppData:
                folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    companyName, appName);
                break;
            case SettingsFolder.AppFolder:
                folderPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                break;
        }
        FileName ??= "usersettings.json";
        FilePath = Path.Combine(folderPath, FileName);
    }
    #endregion Get the settings file path

    #region Read settings file
    /// <summary>
    /// Reads settings from a JSON format settings file
    /// </summary>
    public static void LoadSettings()
    {
        if (File.Exists(FilePath))
        {
            try
            {
                Setting = JsonSerializer.Deserialize<T>(File.ReadAllText(FilePath));
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error reading settings file.\n{ex}",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
            }
        }
        else
        {
            Setting = new T();
        }
    }
    #endregion Read settings file

    #region Save settings
    /// <summary>
    /// Writes settings to settings file
    /// </summary>
    public static void SaveSettings()
    {
        try
        {
            JsonSerializerOptions opts = new()
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(Setting, opts);
            File.WriteAllText(FilePath, json);
        }
        catch (Exception ex)
        {
            _ = MessageBox.Show($"Error saving settings file.\n{ex}",
                                 "Error",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
        }
    }
    #endregion Save settings

    #region Create a new settings file
    /// <summary>
    /// Creates a new, empty JSON settings file
    /// </summary>
    /// <param name="filepath">Complete path and file name</param>
    private static void CreateNewSettingsJson(string filepath)
    {
        try
        {
            if (!Directory.Exists(Path.GetDirectoryName(filepath)))
            {
                _ = Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            }
            File.Create(filepath).Dispose();
            const string braces = "{ }";
            File.WriteAllText(filepath, braces);
        }
        catch (Exception ex)
        {
            _ = MessageBox.Show($"Error creating settings file.\n{ex}",
                                 "Error",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
        }
    }
    #endregion Create a new settings file

    #region List all properties and their values
    public static Dictionary<string, object> ListSettings()
    {
        Type type = typeof(T);
        Dictionary<string, object> properties = new();
        foreach (PropertyInfo p in type.GetProperties())
        {
            properties.Add(p.Name, p.GetValue(Setting));
        }
        return properties;
    }
    #endregion List all properties and their values

    #region Get settings file name
    internal static string GetSettingsFilename()
    {
        return FilePath;
    }
    #endregion Get settings file name
}
