// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    #region MainWindow Instance
    private static readonly MainWindow _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    private const string _getmyip = "GetMyIP";

    #region Relay Commands
    [RelayCommand]
    private static void ViewPermLog()
    {
        if (!string.IsNullOrEmpty(UserSettings.Setting.LogFile) && File.Exists(UserSettings.Setting.LogFile))
        {
            TextFileViewer.ViewTextFile(UserSettings.Setting.LogFile);
        }
        else
        {
            _ = new MDCustMsgBox(GetStringResource("MsgText_Error_FileNotFound"),
                     "Get My IP ERROR",
                     ButtonType.Ok,
                     false,
                     true,
                     _mainWindow,
                     true).ShowDialog();
        }
    }

    [RelayCommand]
    private static async Task TestLogging()
    {
        if (!string.IsNullOrEmpty(UserSettings.Setting.LogFile))
        {
            string json = await IpHelpers.GetExternalInfo();
            IpHelpers.LogIPInfo(json);
            _ = Task.Delay(200);
            TextFileViewer.ViewTextFile(UserSettings.Setting.LogFile);
        }
        else
        {
            _ = new MDCustMsgBox(GetStringResource("MsgText_Error_FileNameMissing"),
                                 "Get My IP ERROR",
                                 ButtonType.Ok,
                                 false,
                                 true,
                                 _mainWindow,
                                 true).ShowDialog();
        }
    }

    [RelayCommand]
    private static void RefreshTooltip()
    {
        CustomToolTip.Instance.ToolTipText = ToolTipHelper.BuildToolTip(false);
        SnackBarMsg.ClearAndQueueMessage(GetStringResource("MsgText_TooltipRefreshed"));
    }

    [RelayCommand]
    private static void OpenAppFolder()
    {
        string filePath = string.Empty;
        try
        {
            filePath = Path.Combine(AppInfo.AppDirectory, "Strings.test.xaml");
            if (File.Exists(filePath))
            {
                _ = Process.Start("explorer.exe", string.Format("/select,\"{0}\"", filePath));
            }
            else
            {
                using Process p = new();
                p.StartInfo.FileName = AppInfo.AppDirectory;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.ErrorDialog = false;
                _ = p.Start();
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, $"Error trying to open {filePath}: {ex.Message}");
            _ = new MDCustMsgBox(GetStringResource("MsgText_Error_FileExplorer"),
                     "Get My IP ERROR",
                     ButtonType.Ok,
                     false,
                     true,
                     _mainWindow,
                     true).ShowDialog();
        }
    }

    [RelayCommand]
    private static void StartWithWindows(RoutedEventArgs e)
    {
        CheckBox cbx = e.Source as CheckBox;
        if (cbx.IsChecked == true)
        {
            if (!RegRun.RegRunEntry(_getmyip))
            {
                string result = RegRun.AddRegEntry(_getmyip, AppInfo.AppPath);
                if (result == "OK")
                {
                    _log.Info(@"Get My IP added to HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    MDCustMsgBox mbox = new(GetStringResource("MsgText_WindowsStartupAdded"),
                                        "Get My IP",
                                        ButtonType.Ok,
                                        true,
                                        true,
                                        _mainWindow,
                                        false);
                    _ = mbox.ShowDialog();
                }
                else
                {
                    _log.Error($"Get My IP add to startup failed: {result}");
                    string msg = string.Format($"{GetStringResource("MsgText_Error_AddToWindowsStartupLine1")}" +
                                                     $"\n\n{GetStringResource("MsgText_Error_AddToWindowsStartupLine2")}");
                    MDCustMsgBox mbox = new(msg,
                                        "Get My IP ERROR",
                                        ButtonType.Ok,
                                        true,
                                        true,
                                        _mainWindow,
                                        true);
                    _ = mbox.ShowDialog();
                }
            }
        }
        else if (RegRun.RegRunEntry(_getmyip))
        {
            string result = RegRun.RemoveRegEntry(_getmyip);
            if (result == "OK")
            {
                _log.Info(@"Get My IP removed from HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                MDCustMsgBox mbox = new(GetStringResource("MsgText_WindowsStartupRemoved"),
                    "Get My IP",
                    ButtonType.Ok,
                    true,
                    true,
                    _mainWindow,
                    false);
                _ = mbox.ShowDialog();
            }
            else
            {
                _log.Error($"Get My IP add to startup failed: {result}");
                string msg = string.Format($"{GetStringResource("MsgText_Error_AddToWindowsStartupLine1")}" +
                                                 $"\n\n{GetStringResource("MsgText_Error_AddToWindowsStartupLine2")}");
                MDCustMsgBox mbox = new(msg,
                                    "Get My IP ERROR",
                                    ButtonType.Ok,
                                    true,
                                    true,
                                    _mainWindow,
                                    true);
                _ = mbox.ShowDialog();
            }
        }
    }

    [RelayCommand]
    public static void UpdateRefresh()
    {
        if (UserSettings.Setting.AutoRefresh)
        {
            RefreshHelpers.StopTimer();
            Task.Delay(50).Wait();
            RefreshHelpers.StartTimer();
        }
    }
    #endregion Relay Commands
}
