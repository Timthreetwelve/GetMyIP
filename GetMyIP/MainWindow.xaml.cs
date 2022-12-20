// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System.Windows.Markup;

namespace GetMyIP
{
    public partial class MainWindow : Window
    {
        #region NLog Instance
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        #endregion NLog Instance

        #region Stopwatch
        private readonly Stopwatch _stopwatch = new();
        #endregion Stopwatch

        public MainWindow()
        {
            InitializeSettings();

            InitializeComponent();

            ReadSettings();
        }

        #region Settings
        private void InitializeSettings()
        {
            _stopwatch.Start();

            UserSettings.Init(UserSettings.SettingsFolder.AppFolder);
        }

        public void ReadSettings()
        {
            // Set NLog configuration
            NLHelpers.NLogConfig();

            // Unhandled exception handler
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Put the version number in the title bar
            Title = $"{AppInfo.AppName} - {AppInfo.TitleVersion}";

            // Startup message in the temp file
            _log.Info($"{AppInfo.AppName} {AppInfo.AppVersion} is starting up");

            // Window position
            Top = UserSettings.Setting.WindowTop;
            Left = UserSettings.Setting.WindowLeft;
            Height = UserSettings.Setting.WindowHeight;
            Width = UserSettings.Setting.WindowWidth;
            Topmost = UserSettings.Setting.KeepOnTop;

            // .NET version, app framework and OS platform
            string version = Environment.Version.ToString();
            _log.Debug($".NET version: {AppInfo.RuntimeVersion.Replace(".NET", "")}  ({version})");
            _log.Debug(AppInfo.Framework);
            _log.Debug(AppInfo.OsPlatform);

            // Light or dark
            SetBaseTheme((ThemeType)UserSettings.Setting.DarkMode);

            // Primary color
            SetPrimaryColor(UserSettings.Setting.PrimaryColor);

            // UI size
            double size = UIScale(UserSettings.Setting.UISize);
            MainGrid.LayoutTransform = new ScaleTransform(size, size);

            // Initial page viewed
            NavigateToPage(UserSettings.Setting.InitialPage);

            // Settings change event
            UserSettings.Setting.PropertyChanged += UserSettingChanged;
        }
        #endregion Settings

        #region Navigation
        private void NavigateToPage(int page)
        {
            NavListBox.SelectedIndex = page;
            _ = NavListBox.Focus();
            switch (page)
            {
            default:
                    _ = MainFrame.Navigate(new Page1());
                    PageTitle.Text = "Internal IP Addresses";
                    break;
                case 1:
                    _ = MainFrame.Navigate(new Page2());
                    PageTitle.Text = "External IP & Geolocation Information";
                    break;
                case 3:
                    _ = MainFrame.Navigate(new Page3());
                    PageTitle.Text = "Settings";
                    break;
                case 4:
                    _ = MainFrame.Navigate(new Page4());
                    PageTitle.Text = "About";
                    break;
                case 6:
                    Application.Current.Shutdown();
                    break;
            }
        }

        private void NavListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavigateToPage(NavListBox.SelectedIndex);
        }
        #endregion Navigation

        #region Set primary color
        private static void SetPrimaryColor(int color)
        {
            PaletteHelper paletteHelper = new();
            ITheme theme = paletteHelper.GetTheme();
            PrimaryColor primary = color switch
            {
                0 => PrimaryColor.Red,
                1 => PrimaryColor.Pink,
                2 => PrimaryColor.Purple,
                3 => PrimaryColor.DeepPurple,
                4 => PrimaryColor.Indigo,
                5 => PrimaryColor.Blue,
                6 => PrimaryColor.LightBlue,
                7 => PrimaryColor.Cyan,
                8 => PrimaryColor.Teal,
                9 => PrimaryColor.Green,
                10 => PrimaryColor.LightGreen,
                11 => PrimaryColor.Lime,
                12 => PrimaryColor.Yellow,
                13 => PrimaryColor.Amber,
                14 => PrimaryColor.Orange,
                15 => PrimaryColor.DeepOrange,
                16 => PrimaryColor.Brown,
                17 => PrimaryColor.Grey,
                18 => PrimaryColor.BlueGrey,
                _ => PrimaryColor.Blue,
            };
            Color primaryColor = SwatchHelper.Lookup[(MaterialDesignColor)primary];
            theme.SetPrimaryColor(primaryColor);
            paletteHelper.SetTheme(theme);
        }
        #endregion Set primary color

        #region UI scale converter
        private static double UIScale(int size)
        {
            return size switch
            {
                0 => 0.90,
                1 => 0.95,
                2 => 1.0,
                3 => 1.05,
                4 => 1.1,
                _ => 1.0,
            };
        }
        #endregion UI scale converter

        #region Window Events
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            InternalIP.GetMyInternalIP();

            ExternalInfo.GetExtInfo();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _stopwatch.Stop();
            _log.Info($"{AppInfo.AppName} is shutting down.  Elapsed time: {_stopwatch.Elapsed:h\\:mm\\:ss\\.ff}");

            // Shut down NLog
            LogManager.Shutdown();

            // Save settings
            UserSettings.Setting.WindowLeft = Math.Floor(Left);
            UserSettings.Setting.WindowTop = Math.Floor(Top);
            UserSettings.Setting.WindowWidth = Math.Floor(Width);
            UserSettings.Setting.WindowHeight = Math.Floor(Height);
            UserSettings.SaveSettings();
        }
        #endregion Window Events

        #region PopupBox button events
        private void BtnLog_Click(object sender, RoutedEventArgs e)
        {
            TextFileViewer.ViewTextFile(NLHelpers.GetLogfileName());
        }

        private void BtnReadme_Click(object sender, RoutedEventArgs e)
        {
            string dir = AppInfo.AppDirectory;
            TextFileViewer.ViewTextFile(Path.Combine(dir, "ReadMe.txt"));
        }

        private void BtnCopyToClip_Click(object sender, RoutedEventArgs e)
        {
            CopytoClipBoard();
        }

        private void BtnSaveText_Click(object sender, RoutedEventArgs e)
        {
            Copyto2TextFile();
        }

        private void BtnShowMap_Click(object sender, RoutedEventArgs e)
        {
            IPInfo lat = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "Latitude");
            IPInfo lon = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "Longitude");
            //string url = $"https://www.latlong.net/c/?lat={lat.Value}&long={lon.Value}";
            string url = $"https://www.google.com/maps/@{lat.Value},{lon.Value},10z";
            //string url = $"https://www.bing.com/maps/default.aspx?cp={lat.Value}~{lon.Value}&lvl=10";
            try
            {
                using Process p = new();
                p.StartInfo.FileName = url;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.ErrorDialog = false;
                _ = p.Start();
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Unable to open default browser");

                _ = MessageBox.Show("Unable to open default browser. See the log file",
                                    "ERROR",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
            }
        }

        #endregion PopupBox button events

        #region Keyboard Events
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.E)
                {
                    NavigateToPage(1);
                }

                if (e.Key == Key.I)
                {
                    NavigateToPage(0);
                }

                if (e.Key == Key.M)
                {
                    switch (UserSettings.Setting.DarkMode)
                    {
                        case 0:
                            UserSettings.Setting.DarkMode = 1;
                            break;
                        case 1:
                            UserSettings.Setting.DarkMode = 2;
                            break;
                        case 2:
                            UserSettings.Setting.DarkMode = 0;
                            break;
                    }
                }
                if (e.Key == Key.Add)
                {
                    EverythingLarger();
                }
                if (e.Key == Key.Subtract)
                {
                    EverythingSmaller();
                }
                if (e.Key == Key.OemComma)
                {
                    NavigateToPage(3);
                }
            }

            if (e.Key == Key.F1)
            {
                NavigateToPage(4);
            }
        }
        #endregion Keyboard Events

        #region Setting change
        private void UserSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyInfo prop = sender.GetType().GetProperty(e.PropertyName);
            object newValue = prop?.GetValue(sender, null);
            _log.Debug($"Setting change: {e.PropertyName} New Value: {newValue}");
            switch (e.PropertyName)
            {
                case nameof(UserSettings.Setting.KeepOnTop):
                    Topmost = (bool)newValue;
                    break;

                case nameof(UserSettings.Setting.IncludeV6):
                    InternalIP.GetMyInternalIP();
                    break;

                case nameof(UserSettings.Setting.IncludeDebug):
                    NLHelpers.SetLogLevel((bool)newValue);
                    break;

                case nameof(UserSettings.Setting.DarkMode):
                    SetBaseTheme((ThemeType)newValue);
                    break;

                case nameof(UserSettings.Setting.PrimaryColor):
                    SetPrimaryColor((int)newValue);
                    break;

                case nameof(UserSettings.Setting.UISize):
                    int size = (int)newValue;
                    double newSize = UIScale(size);
                    MainGrid.LayoutTransform = new ScaleTransform(newSize, newSize);
                    break;
            }
        }
        #endregion Setting change

        #region Smaller/Larger
        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
                return;

            if (e.Delta > 0)
            {
                EverythingLarger();
            }
            else if (e.Delta < 0)
            {
                EverythingSmaller();
            }
        }

        public void EverythingSmaller()
        {
            int size = UserSettings.Setting.UISize;
            if (size > 0)
            {
                size--;
                UserSettings.Setting.UISize = size;
                double newSize = UIScale(size);
                MainGrid.LayoutTransform = new ScaleTransform(newSize, newSize);
            }
        }

        public void EverythingLarger()
        {
            int size = UserSettings.Setting.UISize;
            if (size < 4)
            {
                size++;
                UserSettings.Setting.UISize = size;
                double newSize = UIScale(size);
                MainGrid.LayoutTransform = new ScaleTransform(newSize, newSize);
            }
        }
        #endregion Smaller/Larger

        #region Window Title
        public void WindowTitleVersion()
        {
            Title = $"{AppInfo.AppName} - {AppInfo.TitleVersion}";
        }
        #endregion Window Title

        #region Theme
        /// <summary>
        /// Gets the current theme
        /// </summary>
        /// <returns>Dark or Light</returns>
        internal static string GetSystemTheme()
        {
            BaseTheme? sysTheme = Theme.GetSystemTheme();
            return sysTheme != null ? sysTheme.ToString() : string.Empty;
        }

        /// <summary>
        /// Sets the theme
        /// </summary>
        /// <param name="mode">Light, Dark, Darker or System</param>
        internal static void SetBaseTheme(ThemeType mode)
        {
            //Retrieve the app's existing theme
            PaletteHelper paletteHelper = new();
            ITheme theme = paletteHelper.GetTheme();

            if (mode == ThemeType.System)
            {
                mode = GetSystemTheme()!.Equals("light", StringComparison.Ordinal) ? ThemeType.Light : ThemeType.Dark;
            }

            switch (mode)
            {
                case ThemeType.Light:
                    theme.SetBaseTheme(Theme.Light);
                    theme.Paper = Colors.WhiteSmoke;
                    break;
                case ThemeType.Dark:
                    theme.SetBaseTheme(Theme.Dark);
                    break;
                case ThemeType.Darker:
                    // Set card and paper background colors a bit darker
                    theme.SetBaseTheme(Theme.Dark);
                    theme.CardBackground = (Color)ColorConverter.ConvertFromString("#FF141414");
                    theme.Paper = (Color)ColorConverter.ConvertFromString("#FF202020");
                    break;
                default:
                    theme.SetBaseTheme(Theme.Light);
                    break;
            }

            //Change the app's current theme
            paletteHelper.SetTheme(theme);
        }
        #endregion Theme

        #region Copy to clipboard and text file
        private static void CopytoClipBoard()
        {
            StringBuilder sb = ListView2Sb();
            // Clear the clipboard of any previous text
            Clipboard.Clear();
            // Copy to clipboard
            Clipboard.SetText(sb.ToString());
            _log.Debug("IP information copied to clipboard");
        }

        private static void Copyto2TextFile()
        {
            SaveFileDialog dialog = new()
            {
                Title = "Save",
                Filter = "Text File|*.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                FileName = "IP_Info.txt"
            };
            var result = dialog.ShowDialog();
            if (result == true)
            {
                StringBuilder sb = ListView2Sb();
                File.WriteAllText(dialog.FileName, sb.ToString());
                _log.Debug($"IP information written to {dialog.FileName}");
            }
        }

        private static StringBuilder ListView2Sb()
        {
            //Get ListView contents and separate parameter and value with a tab
            StringBuilder sb = new();
            foreach (IPInfo item in IPInfo.InternalList)
            {
                sb.Append(item.Parameter).Append('\t').AppendLine(item.Value);
            }
            foreach (IPInfo item in IPInfo.GeoInfoList)
            {
                sb.Append(item.Parameter).Append('\t').AppendLine(item.Value);
            }
            return sb;
        }
        #endregion Copy to clipboard and text file

        #region Unhandled Exception Handler
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            _log.Error("Unhandled Exception");
            Exception e = (Exception)args.ExceptionObject;
            _log.Error(e.Message);
            if (e.InnerException != null)
            {
                _log.Error(e.InnerException.ToString());
            }
            _log.Error(e.StackTrace);

            _ = MessageBox.Show("An error has occurred. See the log file",
                                "ERROR",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
        }
        #endregion Unhandled Exception Handler
    }
}