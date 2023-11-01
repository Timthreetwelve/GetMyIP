// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
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
                     null,
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
                                 null,
                                 true).ShowDialog();
        }
    }

    [RelayCommand]
    private static void RefreshTooltip()
    {
        CustomToolTip.Instance.ToolTipText = ToolTipHelper.BuildToolTip();
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
                     null,
                     true).ShowDialog();
        }
    }
    #endregion Relay Commands
}
