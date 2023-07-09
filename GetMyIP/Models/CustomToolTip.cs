// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

public partial class CustomToolTip : ObservableObject
{
    [ObservableProperty]
    private string _toolTipText;

    public static CustomToolTip Instance { get; private set; }

    static CustomToolTip()
    {
       Instance = new CustomToolTip();
    }
}
