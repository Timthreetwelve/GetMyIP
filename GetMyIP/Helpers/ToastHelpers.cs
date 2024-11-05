// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

internal static class ToastHelpers
{
    #region MainWindow Instance
    private static readonly MainWindow? _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region Show toast notification
    /// <summary>
    /// Show a toast notification
    /// </summary>
    /// <param name="title">The title</param>
    /// <param name="message">The message</param>
    public static void ShowToast(string title, string message)
    {
        try
        {
            const NotificationIcon icon = NotificationIcon.Info;
            _ = Application.Current.Dispatcher
                .InvokeAsync(() => _mainWindow!.TbIcon.ShowNotification(title, message, icon));
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error showing toast.");
        }
    }
    #endregion Show toast notification
}
