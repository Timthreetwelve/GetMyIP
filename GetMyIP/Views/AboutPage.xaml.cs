// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Views;

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

    #region Mouse down in ListView
    /// <summary>
    /// Handle mouse down by doing nothing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }
    #endregion Mouse down in ListView
}
