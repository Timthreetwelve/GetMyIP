// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

/// <summary>
/// Provides helper methods for working with network adapters.
/// </summary>
internal static class AdaptersHelpers
{
    #region Methods
    /// <summary>
    /// Gets a list of network adapters based on user preferences.
    /// </summary>
    /// <returns>A list of <see cref="Adapters"/> objects representing the network adapters.</returns>
    public static List<Adapters> GetAdaptersList()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        List<Adapters> adaptersList = [];

        foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            string ipV4 = GetIpAddress(networkInterface, AddressFamily.InterNetwork);
            string ipV6 = GetIpAddress(networkInterface, AddressFamily.InterNetworkV6);

            if (!ShouldIncludeAdapter(networkInterface, ipV4, ipV6))
            {
                continue;
            }

            adaptersList.Add(new Adapters
            {
                Name = networkInterface.Name,
                Description = networkInterface.Description,
                AdapterType = GetAdapterType(networkInterface.NetworkInterfaceType),
                Status = FormatStatus(networkInterface.OperationalStatus),
                PhysicalAddress = FormatMacAddress(networkInterface),
                IpAddress = ipV4,
                IpV6Address = ipV6,
                SubnetMask = GetSubnetMask(networkInterface),
                DefaultGateway = GetDefaultGateway(networkInterface),
                OperationalStatus = networkInterface.OperationalStatus.ToString()
            });
        }
        stopwatch.Stop();
        _log.Debug($"Network adapter discovery took {stopwatch.ElapsedMilliseconds} ms");
        return [.. adaptersList
            .OrderByDescending(a => string.Equals(Convert.ToString(a.OperationalStatus), "Up", StringComparison.OrdinalIgnoreCase))
            .ThenBy(a => a.AdapterType)];
    }

    /// <summary>
    /// Determines whether the specified network interface should be included based on user preferences.
    /// </summary>
    /// <param name="networkInterface">The network interface to evaluate.</param>
    /// <param name="ipV4">The IPv4 address to check.</param>
    /// <param name="ipV6">The IPv6 address to check.</param>
    /// <returns><c>true</c> if the network interface should be included; otherwise, <c>false</c>.</returns>
    private static bool ShouldIncludeAdapter(NetworkInterface networkInterface, string ipV4, string ipV6)
    {
        if (networkInterface.OperationalStatus == OperationalStatus.NotPresent)
        {
            return false;
        }
        if (UserSettings.Setting.ShowAdapterDownOnly && networkInterface.OperationalStatus != OperationalStatus.Down)
        {
            return false;
        }
        if (UserSettings.Setting.ShowAdapterUpOnly && networkInterface.OperationalStatus != OperationalStatus.Up)
        {
            return false;
        }
        if (UserSettings.Setting.ShowEthernetOnly && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
        {
            return false;
        }
        if (UserSettings.Setting.ShowWirelessOnly && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211)
        {
            return false;
        }
        if (UserSettings.Setting.ShowOtherOnly && ((networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet) ||
                                                   (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)))
        {
            return false;
        }
        if (UserSettings.Setting.ShowHasAddressOnly)
        {
            if (string.IsNullOrEmpty(ipV4) && string.IsNullOrEmpty(ipV6))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Gets the IP address of the specified address family from the network interface.
    /// </summary>
    /// <param name="networkInterface">The network interface to query.</param>
    /// <param name="addressFamily">The address family to retrieve (IPv4 or IPv6).</param>
    /// <returns>The IP address as a string, or an empty string if not found.</returns>
    private static string GetIpAddress(NetworkInterface networkInterface, AddressFamily addressFamily)
    {
        IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

        foreach (UnicastIPAddressInformation unicastAddress in ipProperties.UnicastAddresses)
        {
            if (unicastAddress.Address.AddressFamily == addressFamily)
            {
                return unicastAddress.Address.ToString();
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// Formats the MAC address of the specified network interface.
    /// </summary>
    /// <param name="networkInterface">The network interface to query.</param>
    /// <returns>The formatted MAC address as a string, or an empty string if not found.</returns>
    private static string FormatMacAddress(NetworkInterface networkInterface)
    {
        string macAddress = string.Join(":", networkInterface
                            .GetPhysicalAddress()
                            .GetAddressBytes()
                            .Select(static b => b.ToString("X2", CultureInfo.CurrentCulture)));
        return string.IsNullOrWhiteSpace(macAddress) ? string.Empty : macAddress;
    }

    /// <summary>
    /// Formats the operational status of the specified network interface.
    /// </summary>
    /// <param name="status">The operational status to format.</param>
    /// <returns>The localized operational status as a string.</returns>
    private static string FormatStatus(OperationalStatus status)
    {
        return status switch
        {
            OperationalStatus.Up => GetStringResource("AdapterStatus_Up"),
            OperationalStatus.Down => GetStringResource("AdapterStatus_Down"),
            OperationalStatus.Dormant => GetStringResource("AdapterStatus_Dormant"),
            OperationalStatus.LowerLayerDown => GetStringResource("AdapterStatus_LowerLayerDown"),
            OperationalStatus.NotPresent => GetStringResource("AdapterStatus_NotPresent"),
            OperationalStatus.Testing => GetStringResource("AdapterStatus_Testing"),
            _ => GetStringResource("AdapterStatus_Unknown")
        };
    }

    /// <summary>
    /// Gets the string representation of the specified network interface type.
    /// </summary>
    /// <param name="adapterType">The network interface type to query.</param>
    /// <returns>The localized string representation of the network interface type.</returns>
    private static string GetAdapterType(NetworkInterfaceType adapterType)
    {
        return adapterType switch
        {
            NetworkInterfaceType.Ethernet => GetStringResource("AdapterType_Ethernet"),
            NetworkInterfaceType.Wireless80211 => GetStringResource("AdapterType_Wireless"),
            NetworkInterfaceType.Loopback => GetStringResource("AdapterType_Loopback"),
            NetworkInterfaceType.Tunnel => GetStringResource("AdapterType_Tunnel"),
            _ => GetStringResource("AdapterType_Other")
        };
    }

    /// <summary>
    /// Gets the subnet mask of the specified network interface.
    /// </summary>
    /// <param name="networkInterface">The network interface to query.</param>
    /// <returns>The subnet mask as a string, or an empty string if not found.</returns>
    private static string GetSubnetMask(NetworkInterface networkInterface)
    {
        IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
        UnicastIPAddressInformation? unicastAddress = ipProperties.UnicastAddresses
            .FirstOrDefault(static u => u.Address.AddressFamily == AddressFamily.InterNetwork);
        return unicastAddress?.IPv4Mask.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Gets the default gateway of the specified network interface.
    /// </summary>
    /// <param name="networkInterface">The network interface to query.</param>
    /// <returns>The default gateway as a string, or an empty string if not found.</returns>
    private static string GetDefaultGateway(NetworkInterface networkInterface)
    {
        IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
        GatewayIPAddressInformation? gatewayAddress = ipProperties.GatewayAddresses
            .FirstOrDefault(static g => g.Address.AddressFamily == AddressFamily.InterNetwork);
        return gatewayAddress?.Address.ToString() ?? string.Empty;
    }
    #endregion Methods
}
