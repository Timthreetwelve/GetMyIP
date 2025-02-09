// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP;

public partial class MainWindow
{
    public MainWindow()
    {
        SingleInstance.Create(AppInfo.AppName);

        DataContext = new NavigationViewModel();

        ConfigHelpers.InitializeSettings();

        InitializeComponent();

        _ = MainWindowHelpers.GetMyIPStartUp();
    }
}
