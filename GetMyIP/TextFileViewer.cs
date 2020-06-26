// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using TKUtils;

namespace GetMyIP
{
    internal static class TextFileViewer
    {
        #region Text file viewer
        public static void ViewTextFile(string txtfile)
        {
            if (File.Exists(txtfile))
            {
                try
                {
                    using (Process p = new Process())
                    {
                        p.StartInfo.FileName = txtfile;
                        p.StartInfo.UseShellExecute = true;
                        p.StartInfo.ErrorDialog = false;
                        _ = p.Start();
                        WriteLog.WriteTempFile($"Opening {txtfile} in default application");
                    }
                }
                catch (Win32Exception ex)
                {
                    if (ex.NativeErrorCode == 1155)
                    {
                        using (Process p = new Process())
                        {
                            p.StartInfo.FileName = "notepad.exe";
                            p.StartInfo.Arguments = txtfile;
                            p.StartInfo.UseShellExecute = true;
                            p.StartInfo.ErrorDialog = false;
                            _ = p.Start();
                            WriteLog.WriteTempFile($"Opening {txtfile} in Notepad.exe");
                        }
                    }
                    else
                    {
                        _ = MessageBox.Show($"Error reading file {txtfile}\n{ex.Message}", "Watcher Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        WriteLog.WriteTempFile($"* Unable to open {txtfile}");
                        WriteLog.WriteTempFile($"* {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show($"Unable to start default application used to open" +
                        $" {txtfile}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    WriteLog.WriteTempFile($"* Unable to open {txtfile}");
                    WriteLog.WriteTempFile($"* {ex.Message}");
                }
            }
            else
            {
                Debug.WriteLine($">>> File not found: {txtfile}");
            }
        }
        #endregion
    }
}
