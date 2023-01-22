using Microsoft.Win32;

namespace GetMyIP;

/// <summary>
/// Class to get Windows accent (title bar) color from HKCU\SOFTWARE\Microsoft\Windows\DWM.
/// </summary>
internal static class ColorHelper
{
    /// <summary>
    /// Gets the accent color from the registry entry.
    /// </summary>
    /// <returns>SolidColorBrush</returns>
    /// <exception cref="InvalidOperationException"/>
    public static SolidColorBrush GetAccentColor()
    {
        const string _regPath = @"SOFTWARE\Microsoft\Windows\DWM";
        using RegistryKey _dwmKey = Registry.CurrentUser.OpenSubKey(_regPath, RegistryKeyPermissionCheck.ReadSubTree);

        if (_dwmKey is null)
        {
            throw new InvalidOperationException("Registry key not found.");
        }

        object accentColor = _dwmKey.GetValue("AccentColor");
        if (accentColor is int accentDword)
        {
            return ParseDWordColor(accentDword);
        }

        throw new InvalidOperationException("Failed to parse Accent Color.");
    }

    /// <summary>
    /// Parses the color from the registry DWord into SolidColorBrush format.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns></returns>
    private static SolidColorBrush ParseDWordColor(int color)
    {
        byte
            a = (byte)((color >> 24) & 0xFF),
            b = (byte)((color >> 16) & 0xFF),
            g = (byte)((color >> 8) & 0xFF),
            r = (byte)((color >> 0) & 0xFF);

        return new SolidColorBrush(Color.FromArgb(a, r, g, b));
    }

    /// <summary>
    /// Determines whether the specified brush is light or dark.
    /// </summary>
    /// <param name="brush">The brush.</param>
    /// <returns>
    ///   <c>true</c> if the brush dark; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsBrushDark(Brush brush)
    {
        SolidColorBrush b1 = (SolidColorBrush)brush;
        Color col = b1.Color;
        return (col.R * 0.2126) + (col.G * 0.7152) + (col.B * 0.0722) < 255 / 2;
    }
}
