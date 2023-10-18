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
            BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
            binding?.UpdateSource();
        }
    }
    #endregion TextBox key down event

    #region Refresh tool tip text
    private void BtnRefreshToolTip_Click(object sender, RoutedEventArgs e)
    {
        CustomToolTip.Instance.ToolTipText = ToolTipHelper.BuildToolTip();
        SnackBarMsg.ClearAndQueueMessage("Tooltip Refreshed");
    }
    #endregion Refresh tool tip text

    #region Language ComboBox loaded event
    /// <summary>
    /// Handles the Loaded event of the language ComboBox.
    /// </summary>
    private void CbxLanguage_Loaded(object sender, RoutedEventArgs e)
    {
        cbxLanguage.SelectedIndex = LocalizationHelpers.GetLanguageIndex();
    }
    #endregion Language ComboBox loaded event
}
