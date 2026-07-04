// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP;

internal static class BuildInfo
{
    public static readonly string CommitIDString = VersionInfo.GitRevShort;

    public static readonly string CommitIDFullString = VersionInfo.GitRevLong;

    public static readonly string? Prerelease = VersionInfo.VersionPrerelease;

    public static readonly DateTime CommitDateUtc =
    DateTime.Parse(VersionInfo.GitCommitterDate, CultureInfo.InvariantCulture,
        DateTimeStyles.AdjustToUniversal);

    public static string CommitDateStringUtc = $"{CommitDateUtc:f} (UTC)";
    public static string CommitDateStringLocal = $"{CommitDateUtc.ToLocalTime():f} (Local)";

    public static readonly string VersionString = string.IsNullOrWhiteSpace(Prerelease)
        ? VersionInfo.Version
        : $"{VersionInfo.Version}-{Prerelease}";
}
