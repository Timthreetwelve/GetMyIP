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
}
