// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    #region Properties
    /// <summary>
    /// Number of language strings in the test resource dictionary
    /// </summary>
    private static int TestLanguageStrings { get; set; }

    /// <summary>
    /// Uri of the test resource dictionary
    /// </summary>
    private static string? TestLanguageFile { get; set; }

    /// <summary>
    /// Number of language strings in the default resource dictionary
    /// </summary>
    public static int DefaultLanguageStrings { get; private set; }

    /// <summary>
    /// Close the app or minimize to tray
    /// </summary>
    internal static bool ExplicitClose { get; set; }

    /// <summary>
    /// Just here to write a log entry
    /// </summary>
    internal static bool LogOnly { get; set; }

    /// <summary>
    /// Command line arguments
    /// </summary>
    internal static string[] Args { get; private set; } = [];

    /// <summary>
    /// Flag indicating if session is ending
    /// </summary>
    private static volatile bool SessionEndingFlag;
    #endregion Properties

    #region On Startup
    /// <summary>
    /// Override the Startup Event.
    /// </summary>
    /// <param name="e">Startup event arguments</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Unhandled exception handler
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        // Listen for session ending events (logoff/shutdown)
        SessionEnding += App_SessionEnding;

        // Command line arguments
        Args = e.Args;

        // Only allows a single instance of the application to run.
        SingleInstance.Create(AppInfo.AppName);

        // Initialize settings here so that saved language can be accessed below.
        ConfigHelpers.InitializeSettings();

        // Set NLog configuration.
        NLogConfig();

        // Log startup messages.
        MainWindowHelpers.LogStartup();

        // No need to set language if only logging. 
        if (LogOnly)
        { return; }

        // Change language if needed.
        SetLanguage();

        // Enable language testing if requested.
        CheckLanguageTesting();
    }
    #endregion On Startup

    #region Set the UI language
    /// <summary>
    /// Set the UI language.
    /// </summary>
    /// <remarks>
    /// Strings.en-US.xaml is loaded in App.xaml as the fallback language.
    /// Consequently there is no need to explicitly load it in case of an error.
    /// </remarks>
    private void SetLanguage()
    {
        // Get the number of strings in the default language file
        DefaultLanguageStrings = GetTotalDefaultLanguageCount();

        // Resource dictionary for language
        ResourceDictionary LanguageDictionary = [];

        // Log culture info at startup
        _log.Debug($"Startup culture: {LocalizationHelpers.GetCurrentCulture()}  UI: {LocalizationHelpers.GetCurrentUICulture()}");

        // Get the current UI language
        string currentLanguage = Thread.CurrentThread.CurrentUICulture.Name;

        // Check the UseOSLanguage setting. If true try to use the language. Do not change current culture. 
        if (LocalizationHelpers.CheckUseOsLanguage(currentLanguage))
        {
            if (currentLanguage == "en-US")
            {
                LocalizationHelpers.LanguageStrings = DefaultLanguageStrings;
                _log.Debug("Use OS Language option is true. Language is en-US. No need to load language file.");
                return;
            }
            try
            {
                LanguageDictionary.Source = new Uri($"Languages/Strings.{currentLanguage}.xaml", UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(LanguageDictionary);
                _log.Debug($"Use OS Language option is true. Language {currentLanguage} loaded.");
            }
            catch (Exception ex)
            {
                LanguageDictionary.Source = new Uri("Languages/Strings.en-US.xaml", UriKind.RelativeOrAbsolute);
                _log.Warn(ex, $"Language {currentLanguage} could not be located. Defaulting to en-US");
            }
            LocalizationHelpers.ApplyLanguageSettings(LanguageDictionary);
            return;
        }

        // If a language is defined in settings, and it exists in the list of defined languages, set the current culture and language to it.
        if (!string.IsNullOrEmpty(UserSettings.Setting!.UILanguage) &&
            UILanguage.DefinedLanguages.Exists(x => x.LanguageCode == UserSettings.Setting.UILanguage))
        {
            try
            {
                LanguageDictionary.Source = new Uri($"Languages/Strings.{UserSettings.Setting.UILanguage}.xaml", UriKind.RelativeOrAbsolute);
                Thread.CurrentThread.CurrentCulture = new CultureInfo(UserSettings.Setting.UILanguage);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(UserSettings.Setting.UILanguage);
                Resources.MergedDictionaries.Add(LanguageDictionary);
            }
            catch (Exception ex)
            {
                LanguageDictionary.Source = new Uri("Languages/Strings.en-US.xaml", UriKind.RelativeOrAbsolute);
                _log.Warn(ex, $"Error using language \"{UserSettings.Setting.UILanguage}\". Defaulting to en-US");
            }
            LocalizationHelpers.ApplyLanguageSettings(LanguageDictionary);
            return;
        }

        // If language is not found in settings, or the language is not defined in UILanguage.DefinedLanguages, use en-US.
        // Strings.en-US.xaml is loaded in App.xaml therefore there is no need to explicitly load it here.
        LanguageDictionary.Source = new Uri("Languages/Strings.en-US.xaml", UriKind.RelativeOrAbsolute);
        UserSettings.Setting.UILanguage = "en-US";
        ConfigHelpers.SaveSettings();
        _log.Warn("Language defaulting to en-US");
        LocalizationHelpers.ApplyLanguageSettings(LanguageDictionary);
    }
    #endregion Set the UI language

    #region Language testing
    private void CheckLanguageTesting()
    {
        // Language testing
        if (UserSettings.Setting!.LanguageTesting)
        {
            _log.Info("Language testing enabled");
            ResourceDictionary testDict = [];
            string testLanguageFile = Path.Combine(AppInfo.AppDirectory, "Strings.test.xaml");
            if (File.Exists(testLanguageFile))
            {
                try
                {
                    testDict.Source = new Uri(testLanguageFile, UriKind.RelativeOrAbsolute);
                    if (testDict.Source != null)
                    {
                        Resources.MergedDictionaries.Add(testDict);
                        TestLanguageStrings = testDict.Count;
                        TestLanguageFile = testDict.Source.OriginalString;
                        _log.Debug($"{TestLanguageStrings} strings loaded from {TestLanguageFile}");
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex, $"Error loading test language file {TestLanguageFile}");
                    string msg = string.Format(CultureInfo.CurrentCulture,
                                               $"{GetStringResource("MsgText_Error_TestLanguage")}\n\n{ex.Message}\n\n{ex.InnerException}");
                    _ = MessageBox.Show(msg,
                        GetStringResource("MsgText_Error_Caption"),
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
    }
    #endregion Language testing

    #region Unhandled Exception Handler
    /// <summary>
    /// Handles any exceptions that weren't caught elsewhere.
    /// </summary>
    /// <remarks>
    /// This uses default message box.
    /// </remarks>
    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        if (args.ExceptionObject is Exception exception)
        {
            _log.Fatal(exception, "Unhandled exception.");

            string msg = string.IsNullOrWhiteSpace(exception.Message)
                ? GetStringResource("MsgText_Error")
                : exception.Message;
            msg += $"\n\n{GetStringResource("MsgText_Error_SeeLog")}";

            ShowMessageBox(msg);
        }
        else
        {
            if (args.ExceptionObject == null)
            {
                _log.Error("Unhandled exception object is null.");
            }
            else
            {
                _log.Error("Unhandled exception object is not of type Exception. Type: {ExceptionType}", args.ExceptionObject.GetType().FullName);
            }

            string msg = $"{GetStringResource("MsgText_Error")}\n\n{GetStringResource("MsgText_Error_SeeLog")}";
            ShowMessageBox(msg);
        }
    }
    #endregion Unhandled Exception Handler

    #region Session Ending Handler
    /// <summary>
    /// Listens for Windows session ending events (logoff/shutdown).
    /// </summary>
    private static void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
    {
        _log.Info($"Windows session ending: {e.ReasonSessionEnding}");
        SessionEndingFlag = true;
    }
    #endregion Session Ending Handler

    #region Show Message Box
    /// <summary>
    /// Message box display method that handles dispatcher thread access.
    /// Message box is not displayed if session is ending.
    /// </summary>
    private static void ShowMessageBox(string msg)
    {
        if (!CanShowMessageBox())
        {
            return;
        }

        System.Windows.Threading.Dispatcher? dispatcher = Current?.Dispatcher;
        Action showMessageBox = () =>
            MessageBox.Show(msg,
                GetStringResource("MsgText_Error_Caption"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);

        if (dispatcher?.CheckAccess() == true || dispatcher == null)
        {
            showMessageBox();
        }
        else
        {
            dispatcher.Invoke(showMessageBox);
        }
    }

    private static bool CanShowMessageBox()
    {
        if (SessionEndingFlag || Environment.HasShutdownStarted)
        {
            return false;
        }

        System.Windows.Threading.Dispatcher? dispatcher = Current?.Dispatcher;
        return dispatcher == null || (!dispatcher.HasShutdownStarted && !dispatcher.HasShutdownFinished);
    }
    #endregion Show Message Box
}
