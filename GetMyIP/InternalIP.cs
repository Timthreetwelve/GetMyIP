// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region Using directives
using System.Diagnostics;
using System.Net;
using NLog;
#endregion Using directives

namespace GetMyIP
{
    internal static class InternalIP
    {
        #region NLog Instance
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        #endregion NLog Instance

        #region Get Internal IP
        public static void GetMyInternalIP()
        {
            Stopwatch sw = Stopwatch.StartNew();
            IPInfo.InternalList.Clear();

            string host = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(host);

            // Get info for each IPv4 host
            foreach (IPAddress address in hostEntry.AddressList)
            {
                if (address.AddressFamily.ToString() == "InterNetwork")
                {
                    IPInfo.InternalList.Add(new IPInfo("Internal IPv4 Address", address.ToString()));
                    log.Debug($"Internal IPv4 Address is {address}");
                }
            }
            // and optionally for IPv6 host
            if (UserSettings.Setting.IncludeV6)
            {
                foreach (IPAddress address in hostEntry.AddressList)
                {
                    if (address.AddressFamily.ToString() == "InterNetworkV6")
                    {
                        IPInfo.InternalList.Add(new IPInfo("Internal IPv6 Address", address.ToString()));
                        log.Debug($"Internal IPv6 Address is {address}");
                    }
                }
            }
            sw.Stop();
            log.Debug($"Discovering internal addresses took {sw.ElapsedMilliseconds} ms");
        }
        #endregion Get Internal IP
    }
}
