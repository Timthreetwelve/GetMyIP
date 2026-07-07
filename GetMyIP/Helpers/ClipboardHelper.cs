// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

internal static class ClipboardHelper
{
    /// <summary>
    /// Copy to clipboard with retry logic to handle potential exceptions when the clipboard is busy.
    /// </summary>
    public static async Task<bool> CopyTextToClipboardAsync(string? text, int maxRetries = 10, int delayMs = 50)
    {
        if (string.IsNullOrEmpty(text) || maxRetries <= 0)
        {
            return false;
        }

        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null)
        {
            return false;
        }

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                if (dispatcher.CheckAccess())
                {
                    Clipboard.SetText(text);
                }
                else
                {
                    await dispatcher.InvokeAsync(() => Clipboard.SetText(text));
                }

                return true;
            }
            catch (ExternalException) when (attempt < maxRetries)
            {
                await Task.Delay(delayMs).ConfigureAwait(false);
            }
            catch
            {
                return false;
            }
        }

        return false;
    }
}
