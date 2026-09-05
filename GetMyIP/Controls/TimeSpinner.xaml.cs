// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Controls;

/// <summary>
/// A simple spinner control that displays an integer value with a leading zero
/// (e.g. "01", "05") and allows the value to be changed with up/down buttons.
/// Used in place of MaterialDesignThemes NumericUpDown, which does not honor
/// StringFormat on its displayed text.
/// </summary>
public partial class TimeSpinner : UserControl
{
    public TimeSpinner()
    {
        InitializeComponent();
    }

    #region Value dependency property
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(int),
            typeof(TimeSpinner),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, Clamp(value));
    }
    #endregion Value dependency property

    #region Minimum dependency property
    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(
            nameof(Minimum),
            typeof(int),
            typeof(TimeSpinner),
            new PropertyMetadata(0));

    public int Minimum
    {
        get => (int)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }
    #endregion Minimum dependency property

    #region Maximum dependency property
    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(
            nameof(Maximum),
            typeof(int),
            typeof(TimeSpinner),
            new PropertyMetadata(59));

    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }
    #endregion Maximum dependency property

    #region Increment and decrement
    private int Clamp(int value)
    {
        if (value < Minimum)
        {
            return Minimum;
        }
        if (value > Maximum)
        {
            return Maximum;
        }
        return value;
    }

    private void UpButton_Click(object sender, RoutedEventArgs e)
    {
        Value = Clamp(Value + 1);
    }

    private void DownButton_Click(object sender, RoutedEventArgs e)
    {
        Value = Clamp(Value - 1);
    }
    #endregion Increment and decrement
}
