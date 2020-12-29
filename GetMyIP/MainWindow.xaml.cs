// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region Using directives
using GetMyIp;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TKUtils;
#endregion

namespace GetMyIP
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<string, string> ipInfo = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();

            ReadSettings();

            GetMyInternalIP();

            IPInfo2Dict(GetIPInfo(UserSettings.Setting.URL));
        }

        #region Get Internal IP
        public void GetMyInternalIP()
        {
            txtboxInternalIP.Text = string.Empty;
            string host = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(host);

            // Even though this is in a for loop there should only be one address returned
            foreach (IPAddress address in hostEntry.AddressList)
            {
                if (address.AddressFamily.ToString() == "InterNetwork")
                {
                    txtboxInternalIP.Text = address.ToString();
                    WriteLog.WriteTempFile($"Internal IP Address is {address}");
                }
            }
        }
        #endregion

        #region Get External IP
        public string GetIPInfo(string url)
        {
            try
            {
                using (WebClient web = new WebClient())
                {
                    return web.DownloadString(url);
                }
            }
            catch (Exception e)
            {
                _ = MessageBox.Show("*** Error retrieving data ***", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                WriteLog.WriteTempFile($"Error retrieving data - {e.Message} ");
                Application.Current.Shutdown();
                return null;
            }
        }
        #endregion

        #region Deserialize JSON containing IP info
        public void IPInfo2Dict(string json)
        {
            IPGeoLocation info = JsonConvert.DeserializeObject<IPGeoLocation>(json);

            if (string.Equals(info.Status, "success", StringComparison.OrdinalIgnoreCase))
            {
                ipInfo.Add("IP Address", info.IpAddress);
                ipInfo.Add("City", info.City);
                ipInfo.Add("State", info.State);
                ipInfo.Add("Zip Code", info.Zip);
                ipInfo.Add("Longitude", info.Lon.ToString());
                ipInfo.Add("Latitude", info.Lat.ToString());
                ipInfo.Add("Time Zone", info.Timezone);
                ipInfo.Add("ISP", info.Isp);
                WriteLog.WriteTempFile($"External IP Address is {info.IpAddress}.");
            }
            else
            {
                ipInfo.Add("Status", info.Status);
                ipInfo.Add("Message", info.Message);
                WriteLog.WriteTempFile("Get External IP Address failed.");
                WriteLog.WriteTempFile($"Status is {info.Status}. Message is {info.Message}.");
            }

            txtboxEnternalIP.Text = ipInfo["IP Address"];
            dataGrid.ItemsSource = ipInfo;
        }
        #endregion

        #region Read settings
        public void ReadSettings()
        {
            // Initialize and load settings
            UserSettings.Init(UserSettings.AppFolder, UserSettings.DefaultFilename, true);

            // Window position
            Top = UserSettings.Setting.WindowTop;
            Left = UserSettings.Setting.WindowLeft;

            WindowTitleVersion();
        }
        #endregion

        #region Window Events
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            UserSettings.Setting.WindowLeft = Left;
            UserSettings.Setting.WindowTop = Top;

            // save the property settings
            UserSettings.SaveSettings();

            WriteLog.WriteTempFile("GetMyIP is shutting down.");
        }
        #endregion

        #region Menu events
        // Exit
        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Show on map
        private void MnuShowMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string mapURL = string.Format("https://www.latlong.net/c/?lat={0}&long={1}",
                    ipInfo["Latitude"], ipInfo["Longitude"]);
                _ = Process.Start(mapURL);
            }
            catch (Exception ex)
            {
                WriteLog.WriteTempFile($"Unable to open default browser - {ex.Message}");
                _ = MessageBox.Show($"Unable to open default browser.\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Copy to clipboard
        private void MnuCopyToClip_Click(object sender, RoutedEventArgs e)
        {
            CopytoClipBoard();
        }

        // Save to text file
        private void MnuSaveText_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Save",
                Filter = "Text File|*.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                FileName = "IP_Info.txt"
            };
            var result = dialog.ShowDialog();
            if (result == true)
            {
                StringBuilder sb = new StringBuilder();
                _ = sb.Append("Internal IP \t").AppendLine(txtboxInternalIP.Text)
                    .Append("External IP \t").AppendLine(ipInfo["IP Address"])
                    .Append("City \t").AppendLine(ipInfo["City"])
                    .Append("State \t").AppendLine(ipInfo["State"])
                    .Append("Zip Code \t").AppendLine(ipInfo["Zip Code"])
                    .Append("Longitude \t").AppendLine(ipInfo["Longitude"])
                    .Append("Latitude \t").AppendLine(ipInfo["Latitude"])
                    .Append("Time Zone \t").AppendLine(ipInfo["Time Zone"])
                    .Append("ISP \t").AppendLine(ipInfo["ISP"]);

                File.WriteAllText(dialog.FileName, sb.ToString());

                WriteLog.WriteTempFile($"IP information written to {dialog.FileName}");
            }
        }

        // Shade alternating rows
        private void MnuShadeAlt_Checked(object sender, RoutedEventArgs e)
        {
            AltRowShadingOn();
        }

        private void MnuShadeAlt_Unchecked(object sender, RoutedEventArgs e)
        {
            AltRowShadingOff();
        }

        // Show the About window
        public void MnuAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutBox about = new AboutBox
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            about.ShowDialog();
        }

        // View readme
        private void MnuReadMe_Click(object sender, RoutedEventArgs e)
        {
            TextFileViewer.ViewTextFile(@".\ReadMe.txt");
        }
        #endregion Menu

        #region Helper Methods

        private void AltRowShadingOff()
        {
            if (dataGrid != null)
            {
                dataGrid.AlternationCount = 0;
                dataGrid.RowBackground = new SolidColorBrush(Colors.White);
                dataGrid.AlternatingRowBackground = new SolidColorBrush(Colors.White);
                dataGrid.Items.Refresh();
            }
        }

        private void AltRowShadingOn()
        {
            if (dataGrid != null)
            {
                dataGrid.AlternationCount = 1;
                dataGrid.RowBackground = new SolidColorBrush(Colors.White);
                dataGrid.AlternatingRowBackground = new SolidColorBrush(Colors.WhiteSmoke);
                dataGrid.Items.Refresh();
            }
        }

        public void WindowTitleVersion()
        {
            // Get the assembly version
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            // Remove the release (last) node
            string titleVer = version.ToString().Remove(version.ToString().LastIndexOf("."));

            // Set the windows title
            Title = "GetMyIP - " + titleVer;

            WriteLog.WriteTempFile($"GetMyIP version {titleVer}");
        }

        private void CopytoClipBoard()
        {
            // Clear the clipboard
            Clipboard.Clear();

            // Exclude the header row
            dataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;

            // Select all the cells
            dataGrid.SelectAllCells();

            // Execute the copy
            ApplicationCommands.Copy.Execute(null, dataGrid);

            // Unselect the cells
            dataGrid.UnselectAllCells();

            WriteLog.WriteTempFile("IP information copied to clipboard");
        }
        #endregion
    }
}