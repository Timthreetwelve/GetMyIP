﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.ViewModels;

internal partial class NavigationViewModel : ObservableObject
{
    #region Constructor
    public NavigationViewModel()
    {
        if (CurrentViewModel == null)
        {
            NavigateToPage(UserSettings.Setting.InitialPage);
        }
    }
    #endregion Constructor

    #region MainWindow Instance
    private static readonly MainWindow _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region NLog Instance
    private static readonly Logger _log = LogManager.GetLogger("logTemp");
    #endregion NLog Instance

    #region Properties
    [ObservableProperty]
    private object _currentViewModel;

    [ObservableProperty]
    private string _pageTitle;

    [ObservableProperty]
    private static NavigationItem _navItem;
    #endregion Properties

    #region List of navigation items
    public static List<NavigationItem> NavigationViewModelTypes { get; set; } = new List<NavigationItem>
        (new List<NavigationItem>
            {
                new NavigationItem
                {
                    Name="Internal",
                    NavPage = NavPage.Internal,
                    ViewModelType= typeof(InternalInfoViewModel),
                    IconKind=PackIconKind.ComputerClassic,
                    PageTitle="Internal IP Information"
                },
                new NavigationItem
                {
                    Name="External",
                    NavPage = NavPage.External,
                    ViewModelType= typeof(ExternalInfoViewModel),
                    IconKind=PackIconKind.Web,
                    PageTitle="External IP Information"
                },
                new NavigationItem
                {
                    Name="Settings",
                    NavPage=NavPage.Settings,
                    ViewModelType= typeof(SettingsViewModel),
                    IconKind=PackIconKind.SettingsOutline,
                    PageTitle = "Settings"
                },
                new NavigationItem
                {
                    Name="About",
                    NavPage=NavPage.About,
                    ViewModelType= typeof(AboutViewModel),
                    IconKind=PackIconKind.AboutCircleOutline,
                    PageTitle = "About Get My IP"
                },
                new NavigationItem
                {
                    Name="Exit",
                    IconKind=PackIconKind.ExitToApp,
                    IsExit=true
                }
            }
        );
    #endregion List of navigation items

    #region Navigation Methods
    public void NavigateToPage(NavPage page)
    {
        Navigate(FindNavPage(page));
    }

    private static NavigationItem FindNavPage(NavPage page)
    {
        return NavigationViewModelTypes.Find(x => x.NavPage == page);
    }
    #endregion Navigation Methods

    #region Navigate Command
    [RelayCommand]
    internal void Navigate(object param)
    {
        if (param is NavigationItem item)
        {
            if (item.IsExit)
            {
                Application.Current.Shutdown();
            }
            else if (item.ViewModelType is not null)
            {
                PageTitle = item.PageTitle;
                CurrentViewModel = null;
                CurrentViewModel = Activator.CreateInstance((Type)item.ViewModelType);
                NavItem = item;
            }
        }
    }
    #endregion Navigate Command

    #region Show Lat. Long. location in browser
    /// <summary>
    /// Opens the selected browser to the specified latitude and longitude
    /// </summary>
    [RelayCommand]
    private static void ShowMap()
    {
        IPInfo lat = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "Latitude");
        IPInfo lon = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "Longitude");
        string url = UserSettings.Setting.MapProvider switch
        {
            (int)MapProvider.Bing => $"https://www.bing.com/maps/default.aspx?cp={lat.Value}~{lon.Value}&lvl=12",
            (int)MapProvider.LatLong => $"https://www.latlong.net/c/?lat={lat.Value}&long={lon.Value}",
            _ => $"https://www.google.com/maps/@{lat.Value},{lon.Value},12z",
        };
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
    #endregion Show Lat. Long. location in browser

    #region View log and readme
    [RelayCommand]
    private static void ViewLog()
    {
        TextFileViewer.ViewTextFile(NLogHelpers.GetLogfileName());
    }

    [RelayCommand]
    private static void ViewReadMe()
    {
        string dir = AppInfo.AppDirectory;
        TextFileViewer.ViewTextFile(Path.Combine(dir, "ReadMe.txt"));
    }
    #endregion View log and readme

    #region Copy to clipboard and file
    [RelayCommand]
    private static void CopyToClipboard()
    {
        StringBuilder sb = ListToStringBuilder();
        // Clear the clipboard of any existing text
        Clipboard.Clear();
        // Copy to clipboard
        Clipboard.SetText(sb.ToString());
        _log.Debug("IP information copied to clipboard");
    }

    [RelayCommand]
    private static void CopyToFile()
    {
        Microsoft.Win32.SaveFileDialog dialog = new()
        {
            Title = "Save",
            Filter = "Text File|*.txt",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            FileName = "IP_Info.txt"
        };
        var result = dialog.ShowDialog();
        if (result == true)
        {
            StringBuilder sb = ListToStringBuilder();
            File.WriteAllText(dialog.FileName, sb.ToString());
            _log.Debug($"IP information written to {dialog.FileName}");
        }
    }

    private static StringBuilder ListToStringBuilder()
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
    #endregion

    #region Refresh (Used by refresh button and tray context menu)
    [RelayCommand]
    private static async Task RefreshIpInfo()
    {
        await IpHelpers.GetMyInternalIP();
        await IpHelpers.GetExtInfo();
        CustomToolTip.Instance.ToolTipText = ToolTipHelper.BuildToolTip();
    }
    #endregion Refresh (Used by refresh button and tray context menu)

    #region Show Main Window
    [RelayCommand]
    public static void ShowMainWindow()
    {
        MainWindowHelpers.ShowMainWindow();
    }
    #endregion Show Main Window

    #region Exit (Shutdown)
    [RelayCommand]
    public static void Exit()
    {
        Application.Current.Shutdown();
    }
    #endregion Exit (Shutdown)

    #region Key down events
    /// <summary>
    /// Keyboard events
    /// </summary>
    [RelayCommand]
    public static void KeyDown(KeyEventArgs e)
    {
        #region Keys without modifiers
        switch (e.Key)
        {
            case Key.F1:
                {
                    _mainWindow.NavigationListBox.SelectedValue = FindNavPage(NavPage.About);
                    break;
                }
            case Key.F5:
                {
                    Debug.WriteLine("Wire up refresh command");
                    break;
                }
        }
        #endregion Keys without modifiers

        #region Keys with Ctrl
        if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
        {
            switch (e.Key)
            {
                case Key.OemComma:
                    {
                        //NavigateToPage(NavPage.Settings);
                        _mainWindow.NavigationListBox.SelectedValue = FindNavPage(NavPage.Settings);
                        break;
                    }
                case Key.C:
                    {
                        CopyToClipboard();
                        break;
                    }
                case Key.Add:
                    {
                        MainWindowUIHelpers.EverythingLarger();
                        //SnackbarMsg.ClearAndQueueMessage($"Size set to {UserSettings.Setting.UISize}");
                        break;
                    }
                case Key.Subtract:
                    {
                        MainWindowUIHelpers.EverythingSmaller();
                        //SnackbarMsg.ClearAndQueueMessage($"Size set to {UserSettings.Setting.UISize}");
                        break;
                    }
            }
        }
        #endregion Keys with Ctrl

        #region Keys with Ctrl and Shift
        if (e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
        {
            if (e.Key == Key.T)
            {
                switch (UserSettings.Setting.UITheme)
                {
                    case ThemeType.Light:
                        UserSettings.Setting.UITheme = ThemeType.Dark;
                        break;
                    case ThemeType.Dark:
                        UserSettings.Setting.UITheme = ThemeType.Darker;
                        break;
                    case ThemeType.Darker:
                        UserSettings.Setting.UITheme = ThemeType.System;
                        break;
                    case ThemeType.System:
                        UserSettings.Setting.UITheme = ThemeType.Light;
                        break;
                }
                string theme = Converters.EnumDescConverter.GetEnumDescription(UserSettings.Setting.UITheme);
                //SnackbarMsg.ClearAndQueueMessage($"Theme set to {theme}", 2000);
            }
            if (e.Key == Key.C)
            {
                if (UserSettings.Setting.PrimaryColor >= AccentColor.White)
                {
                    UserSettings.Setting.PrimaryColor = AccentColor.Red;
                }
                else
                {
                    UserSettings.Setting.PrimaryColor++;
                }
                string color = Converters.EnumDescConverter.GetEnumDescription(UserSettings.Setting.PrimaryColor);
                //SnackbarMsg.ClearAndQueueMessage($"Accent color set to {color}");
            }
            if (e.Key == Key.S)
            {
                TextFileViewer.ViewTextFile(ConfigHelpers.SettingsFileName);
                //SnackbarMsg.ClearAndQueueMessage("Opening settings file", 2000);
            }
        }
        #endregion
    }
    #endregion Key down events
}
