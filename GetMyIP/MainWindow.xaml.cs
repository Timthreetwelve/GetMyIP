// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GetMyIp;
using Microsoft.Win32;
using Newtonsoft.Json;
using TKUtils;
#endregion

namespace GetMyIP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            UserSettings.Init(UserSettings.AppFolder, UserSettings.DefaultFilename, true);

            InitializeComponent();

            ReadSettings();

            GetMyInternalIP();

            ProcessIPInfo(GetIPInfo(UserSettings.Setting.URL));
        }

        #region Read settings
        public void ReadSettings()
        {
            // Set data grid zoom level
            double curZoom = UserSettings.Setting.GridZoom;
            Grid1.LayoutTransform = new ScaleTransform(curZoom, curZoom);

            // Alternate row shading
            if (UserSettings.Setting.ShadeAltRows)
            {
                AltRowShadingOn();
            }

            WindowTitleVersion();

            // Settings change event
            UserSettings.Setting.PropertyChanged += UserSettingChanged;
        }
        #endregion Read settings

        #region Get Internal IP
        public void GetMyInternalIP()
        {
            //txtboxInternalIP.Text = string.Empty;
            string host = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(host);

            // Even though this is in a for loop there should only be one address returned
            foreach (IPAddress address in hostEntry.AddressList)
            {
                //if (address.AddressFamily.ToString() != null)
                if (address.AddressFamily.ToString() == "InterNetwork")
                {
                    //txtboxInternalIP.Text = address.ToString();
                    IPInfo.InternalList.Add(new IPInfo("Internal IP Address", address.ToString()));
                    WriteLog.WriteTempFile($"Internal IP Address is {address}");
                }
                else if (address.AddressFamily.ToString() == "InterNetworkV6" && UserSettings.Setting.IncludeV6)
                {
                    IPInfo.InternalList.Add(new IPInfo("Internal IPv6 Address ", address.ToString()));
                }
            }
            List<IPInfo> sortedList = IPInfo.InternalList.ToList();
            sortedList.Sort();
            lvInternalInfo.ItemsSource = sortedList;
        }
        #endregion Get Internal IP

        #region Get External IP & Geolocation info
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
        #endregion Get External IP & Geolocation info

        #region Deserialize JSON containing IP info
        public void ProcessIPInfo(string json)
        {
            IPGeoLocation info = JsonConvert.DeserializeObject<IPGeoLocation>(json);

            if (string.Equals(info.Status, "success", StringComparison.OrdinalIgnoreCase))
            {
                IPInfo.GeoInfoList.Add(new IPInfo("External IP Address", info.IpAddress));
                IPInfo.GeoInfoList.Add(new IPInfo("City", info.City));
                IPInfo.GeoInfoList.Add(new IPInfo("State", info.State));
                IPInfo.GeoInfoList.Add(new IPInfo("Zip Code", info.Zip));
                IPInfo.GeoInfoList.Add(new IPInfo("Country", info.Country));
                IPInfo.GeoInfoList.Add(new IPInfo("Continent", info.Continent));
                IPInfo.GeoInfoList.Add(new IPInfo("Longitude", info.Lon.ToString()));
                IPInfo.GeoInfoList.Add(new IPInfo("Latitude", info.Lat.ToString()));
                IPInfo.GeoInfoList.Add(new IPInfo("Time Zone", info.Timezone));
                IPInfo.GeoInfoList.Add(new IPInfo("UTC Offset", ConvertOffset(info.Offset)));
                IPInfo.GeoInfoList.Add(new IPInfo("ISP", info.Isp));
            }
            lvGeoInfo.ItemsSource = IPInfo.GeoInfoList;
        }
        #endregion Deserialize JSON containing IP info

        #region Convert offset from seconds to hours and minutes
        public string ConvertOffset(int offset)
        {
            string neg = "";
            if (offset < 0)
            {
                offset = Math.Abs(offset);
                neg = "-";
            }
            TimeSpan ts = TimeSpan.FromSeconds(offset);
            string hhmm = ts.ToString(@"hh\:mm");
            return neg + hhmm;
        }
        #endregion Convert offset from seconds to hours and minutes

        #region Window Events
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            UserSettings.Setting.WindowLeft = Left;
            UserSettings.Setting.WindowTop = Top;

            // save the property settings
            UserSettings.SaveSettings();

            WriteLog.WriteTempFile("GetMyIP is shutting down.");
        }
        #endregion Window Events

        #region Menu events
        // Exit
        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Show on map
        private void MnuShowMap_Click(object sender, RoutedEventArgs e)
        {
            var lat = IPInfo.GeoInfoList.Find(x => x.Parameter == "Latitude").Value;
            var lon = IPInfo.GeoInfoList.Find(x => x.Parameter == "Longitude").Value;

            try
            {
                string mapURL = string.Format("https://www.latlong.net/c/?lat={0}&long={1}", lat, lon);
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
            Copyto2TextFile();
        }

        private void GridSmaller_Click(object sender, RoutedEventArgs e)
        {
            GridSmaller();
        }

        private void GridLarger_Click(object sender, RoutedEventArgs e)
        {
            GridLarger();
        }

        private void GridReset_Click(object sender, RoutedEventArgs e)
        {
            GridSizeReset();
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

        #region Keyboard Events
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.NumPad0 && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                GridSizeReset();
            }

            if (e.Key == Key.Add && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                GridLarger();
            }

            if (e.Key == Key.Subtract && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                GridSmaller();
            }
            if (e.Key == Key.F1)
            {
                AboutBox about = new AboutBox
                {
                    Owner = Application.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                _ = about.ShowDialog();
            }
            Debug.WriteLine(e.Key);
        }
        #endregion Keyboard Events

        #region Mouse Events
        private void Grid1_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
                return;

            if (e.Delta > 0)
            {
                GridLarger();
            }
            else if (e.Delta < 0)
            {
                GridSmaller();
            }
        }
        #endregion Mouse Events

        #region Setting change
        private void UserSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyInfo prop = sender.GetType().GetProperty(e.PropertyName);
            var newValue = prop?.GetValue(sender, null);
            switch (e.PropertyName)
            {
                case "ShadeAltRows":
                    if ((bool)newValue)
                    {
                        AltRowShadingOn();
                    }
                    else
                    {
                        AltRowShadingOff();
                    }
                    break;

                case "KeepOnTop":
                    Topmost = (bool)newValue;
                    break;

                case "IncludeV6":
                    if (IsVisible)
                    {
                        IPInfo.InternalList.Clear();
                        GetMyInternalIP();
                        lvInternalInfo.Items.Refresh();
                    }
                    break;
            }
            DateTime time = DateTime.Now;
            string debugTime = time.ToString("H:mm:ss");
            Debug.WriteLine($"*** {debugTime} Setting change: {e.PropertyName} New Value: {newValue}");
        }
        #endregion Setting change

        #region Alternate row shading
        private void AltRowShadingOff()
        {
            if (lvGeoInfo != null)
            {
                lvGeoInfo.AlternationCount = 0;
                lvInternalInfo.AlternationCount = 0;
                lvGeoInfo.Items.Refresh();
                lvInternalInfo.Items.Refresh();
            }
        }
        private void AltRowShadingOn()
        {
            if (lvGeoInfo != null)
            {
                lvGeoInfo.AlternationCount = 2;
                lvInternalInfo.AlternationCount = 2;
                lvGeoInfo.Items.Refresh();
                lvInternalInfo.Items.Refresh();
            }
        }
        #endregion Alternate row shading

        #region Grid Zoom
        private void GridSmaller()
        {
            double curZoom = UserSettings.Setting.GridZoom;
            if (curZoom > 0.9)
            {
                curZoom -= .05;
                UserSettings.Setting.GridZoom = Math.Round(curZoom, 2);
            }
            Grid1.LayoutTransform = new ScaleTransform(curZoom, curZoom);
        }
        private void GridLarger()
        {
            double curZoom = UserSettings.Setting.GridZoom;
            if (curZoom < 1.3)
            {
                curZoom += .05;
                UserSettings.Setting.GridZoom = Math.Round(curZoom, 2);
            }
            Grid1.LayoutTransform = new ScaleTransform(curZoom, curZoom);
        }
        private void GridSizeReset()
        {
            UserSettings.Setting.GridZoom = 1.0;
            Grid1.LayoutTransform = new ScaleTransform(1, 1);
        }
        #endregion Grid Zoom

        #region Window Title
        public void WindowTitleVersion()
        {
            Title = $"{AppInfo.AppName} - {AppInfo.TitleVersion}";
        }
        #endregion Window Title

        #region Copy to clipboard and text file
        private void CopytoClipBoard()
        {
            StringBuilder sb = ListView2Sb();
            // Clear the clipboard of any previous text
            Clipboard.Clear();
            // Copy to clipboard
            Clipboard.SetText(sb.ToString());
        }

        private void Copyto2TextFile()
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
                StringBuilder sb = ListView2Sb();
                File.WriteAllText(dialog.FileName, sb.ToString());
                WriteLog.WriteTempFile($"IP information written to {dialog.FileName}");
            }
        }

        private StringBuilder ListView2Sb()
        {
            // Get ListView contents and separate parameter and value with a tab
            StringBuilder sb = new StringBuilder();
            foreach (IPInfo item in lvInternalInfo.Items)
            {
                sb.Append(item.Parameter).Append('\t').AppendLine(item.Value);
            }
            foreach (IPInfo item in lvGeoInfo.Items)
            {
                sb.Append(item.Parameter).Append('\t').AppendLine(item.Value);
            }
            return sb;
        }
        #endregion Copy to clipboard and text file
    }
}