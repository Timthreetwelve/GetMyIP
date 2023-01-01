// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Pages;

/// <summary>
/// Interaction logic for AboutPage.xaml
/// </summary>
public partial class AboutPage : UserControl
{
    public AboutPage()
    {
        InitializeComponent();
    }

    #region Clicked on the GitHub link
    /// <summary>
    /// Opens the default browser to the GitHub page .
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RequestNavigateEventArgs"/> instance containing the event data.</param>
    private void OnNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process p = new();
        p.StartInfo.FileName = e.Uri.AbsoluteUri;
        p.StartInfo.UseShellExecute = true;
        p.Start();
        e.Handled = true;
    }
    #endregion Clicked on the GitHub link

    #region Clicked on the license link
    /// <summary>
    /// Handles the Click event and opens the license file in the default program.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void BtnLicense_Click(object sender, RoutedEventArgs e)
    {
        string dir = AppInfo.AppDirectory;
        TextFileViewer.ViewTextFile(Path.Combine(dir, "License.txt"));
    }
    #endregion Clicked on the license link
}
