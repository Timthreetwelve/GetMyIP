// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

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
                _log.Error(ex, $"Unable to open {textFile}");

                string msg = string.Format($"{GetStringResource("MsgText_Error_OpeningFile")}\n\n {ex.Message}", textFile);
                _ = MessageBox.Show(msg,
                                    "ERROR",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, $"Unable to open {textFile}");

            string msg = string.Format($"{GetStringResource("MsgText_Error_OpeningFile")}\n\n {ex.Message}", textFile);
            _ = MessageBox.Show(msg,
                                "ERROR",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
        }
    }
    #endregion Text file viewer
}
