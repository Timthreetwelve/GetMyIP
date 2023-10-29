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
