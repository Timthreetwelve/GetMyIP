// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

/// <summary>
///  Class for viewing text files. If the file extension is not associated
///  with an application, notepad.exe will be attempted.
/// </summary>
internal static class TextFileViewer
{
    #region Text file viewer
    public static void ViewTextFile(string textFile)
    {
        try
        {
            using Process p = new();
            p.StartInfo.FileName = textFile;
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.ErrorDialog = false;
            _ = p.Start();
            _log.Debug($"Opening {textFile} in default application");
        }
        catch (Win32Exception ex)
        {
            if (ex.NativeErrorCode == 1155)
            {
                using Process p = new();
                p.StartInfo.FileName = "notepad.exe";
                p.StartInfo.Arguments = textFile;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.ErrorDialog = false;
                _ = p.Start();
                _log.Debug($"Opening {textFile} in Notepad.exe");
            }
            else
            {
                string msg = string.Format(CultureInfo.InvariantCulture, MsgTextErrorOpeningFile, textFile);
                _ = MessageBox.Show($"{msg}\n\n{ex.Message}",
                                    GetStringResource("MsgText_Error_Caption"),
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                _log.Error(ex, $"Unable to open {textFile}");
            }
        }
        catch (Exception ex)
        {
            string msg = string.Format(CultureInfo.InvariantCulture, MsgTextErrorOpeningFile, textFile);
            _ = MessageBox.Show($"{msg}\n\n{ex.Message}",
                                GetStringResource("MsgText_Error_Caption"),
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            _log.Error(ex, $"Unable to open {textFile}");
        }
    }
    #endregion Text file viewer
}
