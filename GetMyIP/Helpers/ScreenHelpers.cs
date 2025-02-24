// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System.Windows.Forms;
using System.Windows.Interop;

namespace GetMyIP.Helpers;

internal static class ScreenHelpers
{
    #region Center the window on the screen
    /// <summary>
    /// Centers the window on the screen.
    /// Inspired by https://stackoverflow.com/a/65755995/15237757
    /// </summary>
    public static void CenterTheWindow(Window? window)
    {
        if (window is null)
        {
            return;
        }

        PresentationSource source = PresentationSource.FromVisual(window)!;
        double dpiScale = source.CompositionTarget?.TransformFromDevice.M11 ?? 1.0;

        IntPtr hwnd = new WindowInteropHelper(window).Handle;
        Screen currentMonitor = Screen.FromHandle(hwnd);
        System.Drawing.Rectangle workingArea = currentMonitor.WorkingArea;

        window.Left = (dpiScale * workingArea.Left) + (((dpiScale * workingArea.Width) - window.Width) / 2);
        window.Top = (dpiScale * workingArea.Top) + (((dpiScale * workingArea.Height) - window.Height) / 2);
    }
    #endregion Center the window on the screen

    #region Reposition off-screen window back to the desktop
    /// <summary>
    /// Keep the window on the screen.
    /// </summary>
    public static void KeepWindowOnScreen(Window? window)
    {
        if (window is null || (UserSettings.Setting!.RestoreToCenter && UserSettings.Setting.StartCentered))
        {
            return;
        }

        // the SystemParameters properties work better for this method than Screen properties.
        if (window.Top < SystemParameters.VirtualScreenTop)
        {
            window.Top = SystemParameters.VirtualScreenTop;
        }

        if (window.Left < SystemParameters.VirtualScreenLeft)
        {
            window.Left = SystemParameters.VirtualScreenLeft;
        }

        if (window.Left + window.Width > SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth)
        {
            window.Left = SystemParameters.VirtualScreenWidth + SystemParameters.VirtualScreenLeft - window.Width;
        }

        if (window.Top + window.Height > SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight)
        {
            window.Top = SystemParameters.WorkArea.Size.Height + SystemParameters.VirtualScreenTop - window.Height;
        }
    }
    #endregion Reposition off-screen window back to the desktop
}
