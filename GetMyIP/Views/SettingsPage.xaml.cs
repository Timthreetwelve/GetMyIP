// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Views;

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
            BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop)!;
            binding.UpdateSource();
        }
    }
    #endregion TextBox key down event

    #region Language ComboBox loaded event
    /// <summary>
    /// Handles the Loaded event of the language ComboBox.
    /// </summary>
    private void CbxLanguage_Loaded(object sender, RoutedEventArgs e)
    {
        CbxLanguage.SelectedIndex = LocalizationHelpers.GetLanguageIndex();
    }
    #endregion Language ComboBox loaded event

    #region Save settings when navigating away from the Settings page
    /// <summary>
    /// Since this app can be long running and can be terminated by Windows shutdown,
    /// it is possible that settings may not be saved. Therefore, settings will be
    /// saved every time the Settings page is unloaded.
    /// </summary>
    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        ConfigHelpers.SaveSettings();
    }
    #endregion Save settings when navigating away from the Settings page
}
