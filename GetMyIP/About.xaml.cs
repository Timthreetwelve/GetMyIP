// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System.Diagnostics;
using System.Reflection;
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

            AssemblyInfo asInfo = new AssemblyInfo(Assembly.GetEntryAssembly());
            tbVersion.Text = asInfo.Version.Remove(asInfo.Version.LastIndexOf("."));
            tbCopyright.Text = asInfo.Copyright.Replace("Copyright ", "");
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
