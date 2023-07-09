// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Views;

/// <summary>
/// This is the Internal IP page
/// </summary>
public partial class InternalInfoPage : UserControl
{
    public InternalInfoPage()
    {
        InitializeComponent();

        SetSpacing((Spacing)UserSettings.Setting.RowSpacing);

        UserSettings.Setting.PropertyChanged += UserSettingChanged;
    }

    #region Setting change event handler
    /// <summary>
    /// Handles a change in user settings. In this case it is just row spacing.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
    private void UserSettingChanged(object sender, PropertyChangedEventArgs e)
    {
        PropertyInfo prop = sender.GetType().GetProperty(e.PropertyName);
        object newValue = prop?.GetValue(sender, null);
        switch (e.PropertyName)
        {
            case nameof(UserSettings.Setting.RowSpacing):
                SetSpacing((Spacing)newValue);
                break;
        }
    }
    #endregion Setting change event handler

    #region Set the row spacing
    /// <summary>
    /// Sets the padding & margin around the items in the DataGrid
    /// </summary>
    /// <param name="spacing"></param>
    private void SetSpacing(Spacing spacing)
    {
        switch (spacing)
        {
            case Spacing.Compact:
                DataGridAssist.SetCellPadding(DGInt, new Thickness(15, 3, 15, 3));
                break;
            case Spacing.Comfortable:
                DataGridAssist.SetCellPadding(DGInt, new Thickness(15, 5, 15, 5));
                break;
            case Spacing.Spacious:
                DataGridAssist.SetCellPadding(DGInt, new Thickness(15, 8, 15, 8));
                break;
        }
    }
    #endregion Set the row spacing
}
