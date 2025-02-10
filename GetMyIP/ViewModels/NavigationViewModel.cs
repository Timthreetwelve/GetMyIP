// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.ViewModels;

internal sealed partial class NavigationViewModel : ObservableObject
{
    #region Constructor
    public NavigationViewModel()
    {
        if (CurrentViewModel is null)
        {
            Navigate(FindNavPage(UserSettings.Setting!.InitialPage));
        }
    }
    #endregion Constructor

    #region MainWindow Instance
    private static readonly MainWindow? _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region Properties
    [ObservableProperty]
    private object? _currentViewModel;

    [ObservableProperty]
    private string? _pageTitle;

    [ObservableProperty]
    private static NavigationItem? _navItem;
    #endregion Properties

    #region List of navigation items
    public static List<NavigationItem> NavigationViewModelTypes { get; set; } =
            [
                new ()
                {
                    Name = GetStringResource("NavItem_Internal"),
                    NavPage = NavPage.Internal,
                    ViewModelType = typeof(InternalInfoViewModel),
                    IconKind = PackIconKind.ComputerClassic,
                    PageTitle =  GetStringResource("NavTitle_Internal")
                },
                new ()
                {
                    Name = GetStringResource("NavItem_External"),
                    NavPage = NavPage.External,
                    ViewModelType = typeof(ExternalInfoViewModel),
                    IconKind = PackIconKind.Web,
                    PageTitle = GetStringResource("NavTitle_External")
                },
                new ()
                {
                    Name = GetStringResource("NavItem_Settings"),
                    NavPage=NavPage.Settings,
                    ViewModelType= typeof(SettingsViewModel),
                    IconKind=PackIconKind.SettingsOutline,
                    PageTitle = GetStringResource("NavTitle_Settings")
                },
                new ()
                {
                    Name = GetStringResource("NavItem_About"),
                    NavPage=NavPage.About,
                    ViewModelType= typeof(AboutViewModel),
                    IconKind=PackIconKind.AboutCircleOutline,
                    PageTitle = GetStringResource("NavTitle_About")
                },
                new ()
                {
                    Name = GetStringResource("NavItem_Exit"),
                    IconKind = PackIconKind.ExitToApp,
                    IsExit = true
                }
        ];
    #endregion List of navigation items

    #region Navigation Methods
    private static NavigationItem FindNavPage(NavPage page)
    {
        return NavigationViewModelTypes.Find(x => x.NavPage == page)!;
    }
    #endregion Navigation Methods

    #region Navigate Command
    [RelayCommand]
    private void Navigate(object param)
    {
        if (param is NavigationItem item)
        {
            if (item.IsExit)
            {
                App.ExplicitClose = true;
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
        IPInfo lat = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == GetStringResource("External_Latitude"))!;
        IPInfo lon = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == GetStringResource("External_Longitude"))!;
        if (lat.Value is not null && lon.Value is not null)
        {
            string url = UserSettings.Setting!.MapProvider switch
            {
                (int)MapProvider.Bing => $"https://www.bing.com/maps/default.aspx?cp={lat.Value}~{lon.Value}&lvl=12",
                (int)MapProvider.LatLong => $"https://www.latlong.net/c/?lat={lat.Value}&long={lon.Value}",
                (int)MapProvider.OSM => $"https://www.openstreetmap.org/?mlat={lat.Value}&mlon={lon.Value}",
                _ => $"https://www.google.com/maps/@{lat.Value},{lon.Value},12z",
            };
            try
            {
                SnackBarMsg.ClearAndQueueMessage(GetStringResource("MsgText_BrowserOpening"));
                using Process p = new();
                p.StartInfo.FileName = url;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.ErrorDialog = false;
                _ = p.Start();
            }
            catch (Exception ex)
            {
                _log.Error(ex, GetStringResource("MsgText_BrowserUnableToOpen"));

                _ = MessageBox.Show(GetStringResource("MsgText_BrowserUnableToOpen"),
                                    "Get My IP ERROR",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
            }
        }
        else
        {
            _ = new MDCustMsgBox(GetStringResource("MsgText_Error_LatLonUnavailable"),
                "Get My IP",
                ButtonType.Ok,
                false,
                true,
                _mainWindow,
                true).ShowDialog();
        }
    }
    #endregion Show Lat. Long. location in browser

    #region View log and readme
    [RelayCommand]
    public static void ViewLog()
    {
        TextFileViewer.ViewTextFile(GetLogfileName());
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

        try
        {
            if (ClipboardHelper.CopyTextToClipboard(sb.ToString()))
            {
                _log.Debug($"IP information copied to clipboard. ({sb.Length} bytes)");
                SnackBarMsg.ClearAndQueueMessage(GetStringResource("MsgText_CopiedToClipboard"));
            }
            else
            {
                _log.Error("CopyToClipboard failed.");
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "CopyToClipboard failed.");
        }
    }

    [RelayCommand]
    private static void CopyToFile()
    {
        try
        {
            SaveFileDialog dialog = new()
            {
                Filter = "Text File|*.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                FileName = "IP_Info.txt"
            };
            var result = dialog.ShowDialog();
            if (result == true)
            {
                StringBuilder sb = ListToStringBuilder();
                File.WriteAllText(dialog.FileName, sb.ToString());
                _log.Debug($"IP information written to {dialog.FileName}. ({sb.Length} bytes)");
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "CopyToFile failed.");
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
    #endregion Copy to clipboard and file

    #region Refresh (Used by refresh button and tray context menu)
    /// <summary>
    /// Refreshes internal and external IP info
    /// </summary>
    /// <returns>Task</returns>
    [RelayCommand]
    public static async Task RefreshIpInfo()
    {
        _log.Debug("Refreshing IP information");
        string returnedJson = await IpHelpers.GetAllInfoAsync();
        IpHelpers.ProcessProvider(returnedJson, false);
        CustomToolTip.Instance.ToolTipText = ToolTipHelper.BuildToolTip(true);
        if (_mainWindow!.Visibility == Visibility.Visible)
        {
            SnackBarMsg.ClearAndQueueMessage(GetStringResource("MsgText_Refreshed"));
        }
    }
    #endregion Refresh (Used by refresh button and tray context menu)

    #region Refresh external IP address info
    /// <summary>
    /// For use by the refresh option of the tray icon context menu
    /// and the periodic refresh option.
    /// </summary>
    /// <returns>Task</returns>
    public static async Task RefreshExternalAsync()
    {
        _log.Debug("Refreshing external IP information");
        string returnedJson = await IpHelpers.GetExternalAsync();
        IpHelpers.ProcessProvider(returnedJson, true);
        CustomToolTip.Instance.ToolTipText = ToolTipHelper.BuildToolTip(true);
        if (_mainWindow!.Visibility == Visibility.Visible)
        {
            SnackBarMsg.ClearAndQueueMessage(GetStringResource("MsgText_Refreshed"));
        }
    }
    #endregion Refresh external IP address info

    #region Show Main Window
    [RelayCommand]
    private static void ShowMainWindow()
    {
        MainWindowHelpers.ShowMainWindow();
    }
    #endregion Show Main Window

    #region Exit (Shutdown)
    [RelayCommand]
    private static void Exit()
    {
        App.ExplicitClose = true;
        Application.Current.Shutdown();
    }
    #endregion Exit (Shutdown)

    #region Check for new release
    [RelayCommand]
    private static async Task CheckReleaseAsync()
    {
        await GitHubHelpers.CheckRelease();
    }
    #endregion Check for new release

    #region Right mouse button
    /// <summary>
    /// Copy (nearly) any text in a TextBlock to the clipboard on right mouse button up.
    /// </summary>
    [RelayCommand]
    private static void RightMouseUp(MouseButtonEventArgs e)
    {
        if (e.OriginalSource is not TextBlock text)
        {
            return;
        }

        try
        {
            if (ClipboardHelper.CopyTextToClipboard(text.Text))
            {
                SnackBarMsg.ClearAndQueueMessage(GetStringResource("MsgText_CopiedToClipboard"));
                _log.Debug($"{text.Text.Length} bytes copied to the clipboard");
            }

            DataGridRow dgr = MainWindowHelpers.FindParent<DataGridRow>(text);
            dgr.IsSelected = false;
            DataGrid dg = MainWindowHelpers.FindParent<DataGrid>(dgr);
            dg.Items.Refresh();
        }
        catch (Exception ex)
        {
            _log.Error(ex, $"Right-click event handler failed. {ex.Message}");
        }
    }
    #endregion Right mouse button

    #region Key down events
    /// <summary>
    /// Keyboard events
    /// </summary>
    [RelayCommand]
    private static void KeyDown(KeyEventArgs e)
    {
        #region Keys without modifiers
        if (e.KeyboardDevice.Modifiers == ModifierKeys.None)
        {
            switch (e.Key)
            {
                case Key.F1:
                    {
                        _mainWindow!.NavigationListBox.SelectedValue = FindNavPage(NavPage.About);
                        break;
                    }
                case Key.F5:
                    {
                        _ = RefreshIpInfo();
                        break;
                    }
            }
        }
        #endregion Keys without modifiers

        #region Alt + F4
        if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.F4)
        {
            App.ExplicitClose = true;
            Application.Current.Shutdown();
        }
        #endregion Alt + F4

        #region Keys with Ctrl
        if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
        {
            switch (e.Key)
            {
                case Key.OemComma:
                    {
                        _mainWindow!.NavigationListBox.SelectedValue = FindNavPage(NavPage.Settings);
                        break;
                    }
                case Key.C:
                    {
                        CopyToClipboard();
                        break;
                    }
                case Key.Add:
                case Key.OemPlus:
                    {
                        MainWindowHelpers.EverythingLarger();
                        ShowUIChangeMessage("size");
                        break;
                    }
                case Key.Subtract:
                case Key.OemMinus:
                    {
                        MainWindowHelpers.EverythingSmaller();
                        ShowUIChangeMessage("size");
                        break;
                    }
            }
        }
        #endregion Keys with Ctrl

        #region Keys with Ctrl and Shift
        if (e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
        {
            switch (e.Key)
            {
                case Key.C:
                    {
                        if (UserSettings.Setting!.PrimaryColor >= AccentColor.White)
                        {
                            UserSettings.Setting.PrimaryColor = AccentColor.Red;
                        }
                        else
                        {
                            UserSettings.Setting.PrimaryColor++;
                        }
                        ShowUIChangeMessage("color");
                        break;
                    }
                case Key.D:
                    {
                        ConfigHelpers.DumpSettings();
                        ViewLog();
                        e.Handled = true;
                        break;
                    }
                case Key.F:
                    {
                        using Process p = new();
                        p.StartInfo.FileName = AppInfo.AppDirectory;
                        p.StartInfo.UseShellExecute = true;
                        p.StartInfo.ErrorDialog = false;
                        _ = p.Start();
                        break;
                    }
                case Key.K:
                    CompareLanguageDictionaries();
                    ViewLog();
                    e.Handled = true;
                    break;
                case Key.P:
                    {
                        if (UserSettings.Setting!.InfoProvider >= PublicInfoProvider.IP2Location)
                        {
                            UserSettings.Setting.InfoProvider = PublicInfoProvider.IpApiCom;
                        }
                        else
                        {
                            UserSettings.Setting.InfoProvider++;
                        }
                        break;
                    }
                case Key.R when UserSettings.Setting?.RowSpacing >= Spacing.Wide:
                    UserSettings.Setting.RowSpacing = Spacing.Compact;
                    break;
                case Key.R:
                    if (UserSettings.Setting?.RowSpacing >= Spacing.Wide)
                    {
                        UserSettings.Setting.RowSpacing = Spacing.Compact;
                    }
                    else
                    {
                        UserSettings.Setting!.RowSpacing++;
                    }
                    e.Handled = true;
                    break;
                case Key.S:
                    TextFileViewer.ViewTextFile(ConfigHelpers.SettingsFileName!);
                    break;
                case Key.T:
                    {
                        switch (UserSettings.Setting!.UITheme)
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
                        ShowUIChangeMessage("theme");
                        break;
                    }
                case Key.Add:
                case Key.OemPlus:
                    if (UserSettings.Setting!.SelectedFontSize < 24)
                    {
                        UserSettings.Setting.SelectedFontSize++;
                    }
                    ShowUIChangeMessage("fontSize");
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    if (UserSettings.Setting!.SelectedFontSize > 8)
                    {
                        UserSettings.Setting.SelectedFontSize--;
                    }
                    ShowUIChangeMessage("fontSize");
                    break;
            }
        }
        #endregion Keys with Ctrl and Shift
    }
    #endregion Key down events

    #region Show snack bar message for UI changes
    private static void ShowUIChangeMessage(string messageType)
    {
        CompositeFormat? composite = null;
        string messageVar = string.Empty;

        switch (messageType)
        {
            case "size":
                composite = MsgTextUISizeSet;
                messageVar = EnumDescConverter.GetEnumDescription(UserSettings.Setting!.UISize);
                break;
            case "theme":
                composite = MsgTextUIThemeSet;
                messageVar = EnumDescConverter.GetEnumDescription(UserSettings.Setting!.UITheme);
                break;
            case "color":
                composite = MsgTextUIColorSet;
                messageVar = EnumDescConverter.GetEnumDescription(UserSettings.Setting!.PrimaryColor);
                break;
            case "fontSize":
                composite = MsgTextFontSizeSet;
                messageVar = UserSettings.Setting!.SelectedFontSize.ToString(CultureInfo.CurrentCulture);
                break;
        }

        if (composite is not null)
        {
            string message = string.Format(CultureInfo.InvariantCulture, composite!, messageVar);
            SnackBarMsg.ClearAndQueueMessage(message, 2000);
        }
    }
    #endregion Show snack bar message for UI changes
}
