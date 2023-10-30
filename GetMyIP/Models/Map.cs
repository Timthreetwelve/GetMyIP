// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

public partial class Map : ObservableObject
{
    [ObservableProperty]
    private static bool _canMap;

    public static Map Instance { get; private set; }

    static Map()
    {
        Instance = new Map();
    }
}
