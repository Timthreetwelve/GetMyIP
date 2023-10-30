﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP;

public partial class MainWindow
{
    public MainWindow()
    {
        ConfigHelpers.InitializeSettings();

        InitializeComponent();

        MainWindowHelpers.GetMyIPStartUp();
    }
}