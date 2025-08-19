// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

internal static class MessageHelpers
{
    /// <summary>
    /// Enums to define where error originated. Used by <see cref="ShowErrorMessage"/>.
    /// </summary>
    public enum ErrorSource
    {
        internalIP = 1,
        externalIP = 2,
        both = 3
    }


    /// <summary>
    /// Shows an error message in a MessageBox.
    /// </summary>
    /// <param name="errorMsg">The error message.</param>
    /// <param name="source">Source of the error (internal or external).</param>
    /// <param name="clear">Clear the list before adding the message.</param>
    public static void ShowErrorMessage(string errorMsg, ErrorSource source, bool clear)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            string message = GetStringResource("External_Message");
            switch (source)
            {
                case ErrorSource.externalIP:
                    if (clear)
                    {
                        IPInfo.GeoInfoList.Clear();
                    }
                    IPInfo.GeoInfoList.Add(new IPInfo(message, errorMsg));
                    break;
                case ErrorSource.internalIP:
                    if (clear)
                    {
                        IPInfo.InternalList.Clear();
                    }
                    IPInfo.InternalList.Add(new IPInfo(message, errorMsg));
                    break;
                default:
                    if (clear)
                    {
                        IPInfo.InternalList.Clear();
                        IPInfo.GeoInfoList.Clear();
                    }
                    IPInfo.InternalList.Add(new IPInfo(message, errorMsg));
                    IPInfo.GeoInfoList.Add(new IPInfo(message, errorMsg));
                    break;
            }
        });
    }
}
