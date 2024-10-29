// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

internal static class ToastHelpers
{
    #region Show toast with two lines of text
    /// <summary>
    /// Show a toast notification
    /// </summary>
    /// <param name="line1">First line of text</param>
    /// <param name="line2">Second line of text</param>
    public static void ShowToast(string line1, string line2)
    {
        try
        {
            string imagePath = Path.GetFullPath(Path.Combine(AppInfo.AppDirectory, "Images/IP.png"));
            ToastContentBuilder toast = new();
            _ = toast.SetToastScenario(ToastScenario.Default)
                     .AddAttributionText("via Get My IP")
                     .AddText(line1)
                     .AddText(line2);
            // Only add image if it exists!
            if (File.Exists(imagePath))
            {
                toast.AddAppLogoOverride(new Uri(imagePath));
            }
            toast.Show();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error building/showing toast");
            throw new InvalidOperationException(ex.Message);
        }
    }
    #endregion Show toast with two lines of text

    #region Show toast with one line of text
    /// <summary>
    /// Show a toast notification
    /// </summary>
    /// <param name="line1">Only line of text</param>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static void ShowToast(string line1)
    {
        try
        {
            string imagePath = Path.GetFullPath(Path.Combine(AppInfo.AppDirectory, "Images/IP.png"));
            ToastContentBuilder toast = new();
            _ = toast.SetToastScenario(ToastScenario.Default)
                     .AddAttributionText("via Get My IP")
                     .AddText(line1);
            // Only add image if it exists!
            if (File.Exists(imagePath))
            {
                toast.AddAppLogoOverride(new Uri(imagePath));
            }
            toast.Show();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error building/showing toast");
            throw new InvalidOperationException(ex.Message);
        }
    }
    #endregion Show toast with one line of text
}
