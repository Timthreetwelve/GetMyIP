// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Pages;

/// <summary>
/// Interaction logic for SettingsPage.xaml
/// </summary>
public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    #region TextBox key down event
    /// <summary>Handles the KeyDown event of the TextBox control.</summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        // Update property when enter is pressed
        if (e.Key == Key.Enter)
        {
            // https://stackoverflow.com/a/13289118
            TextBox tBox = (TextBox)sender;
            DependencyProperty prop = TextBox.TextProperty;
            BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
            binding?.UpdateSource();
        }
    }
    #endregion TextBox key down event

    #region Routed event goodies
    /// <summary>
    /// Handles the CanExecute event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
    private void Log_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = !string.IsNullOrEmpty(UserSettings.Setting.LogFile);
    }

    /// <summary>
    /// Handles the Executed event of the TestLog control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="ExecutedRoutedEventArgs"/> instance containing the event data.</param>
    private async void TestLog_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        ExternalInfo.LogIPInfo();
        await Task.Delay(200);
        TextFileViewer.ViewTextFile(UserSettings.Setting.LogFile);
    }

    /// <summary>
    /// Handles the Executed event of the ViewLog control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="ExecutedRoutedEventArgs"/> instance containing the event data.</param>
    private void ViewLog_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        TextFileViewer.ViewTextFile(UserSettings.Setting.LogFile);
    }
    #endregion Routed event goodies
}
