// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

/// <summary>
/// Class for checking network connectivity
/// </summary>
internal static class ConnectivityHelpers
{
    /// <summary>
    /// Determines whether [is connected to network].
    /// </summary>
    /// <returns>
    ///   <c>true</c> if [is connected to network]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsConnectedToNetwork()
    {
        return NetworkInterface.GetIsNetworkAvailable();
    }
}
