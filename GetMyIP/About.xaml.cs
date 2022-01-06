// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace GetMyIP
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class AboutBox : Window
    {
        public AboutBox()
        {
            InitializeComponent();

            tbVersion.Text = AppInfo.AppFileVersion;
            tbCopyright.Text = AppInfo.AppCopyright;
        }

        private void OnNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
