// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP;

internal static class TextFileViewer
{
    private static readonly Logger _log = LogManager.GetLogger("logTemp");

    #region Text file viewer
    public static void ViewTextFile(string txtfile)
    {
        if (File.Exists(txtfile))
        {
            try
            {
                using Process p = new();
                p.StartInfo.FileName = txtfile;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.ErrorDialog = false;
                _ = p.Start();
                _log.Debug($"Opening {txtfile} in default application");
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == 1155)
                {
                    using Process p = new();
                    p.StartInfo.FileName = "notepad.exe";
                    p.StartInfo.Arguments = txtfile;
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.ErrorDialog = false;
                    _ = p.Start();
                    _log.Debug($"Opening {txtfile} in Notepad.exe");
                }
                else
                {
                    _log.Error(ex, $"Unable to open {txtfile}");

                    _ = MessageBox.Show($"Unable to open {txtfile}. See the log file",
                                        "ERROR",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Unable to open {txtfile}");

                _ = MessageBox.Show($"Unable to open {txtfile}. See the log file",
                                    "ERROR",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
            }
        }
        else
        {
            _log.Error($"File not found {txtfile}");

            _ = MessageBox.Show($"File not found {txtfile}. See the log file",
                                "ERROR",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
        }
    }
    #endregion Text file viewer
}
